using System;
using UnityEngine;

#if PTSDK_IAP
using UnityEngine.Purchasing;
#if PTSDK_IAP_VALIDATION
using UnityEngine.Purchasing.Security;
using com.adjust.sdk.purchase;



namespace PTSDKit
{
    public class VerificationRequest
    {
        public static bool enableModuleLogs;
        public static string LogColorCode;

        public bool processing;
        public Action<bool> onValidationComplete;

        public VerificationRequest(Product product, Action<bool> onVerificationComplete)
        {
            if (Application.isEditor)
            {
                if (enableModuleLogs) "Verification skipped for editor".Log(LogColorCode);
                onVerificationComplete?.Invoke(true);
                return;
            }
            CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            IPurchaseReceipt[] preciepts = validator.Validate(product.receipt);
#if UNITY_ANDROID
            GooglePlayReceipt gReceipt = null;
            IAP_RecieptObject_Android recieptObject = null;
            try
            {
                recieptObject = JsonUtility.FromJson<IAP_RecieptObject_Android>(product.receipt);
            }
            catch (Exception ex1)
            {
                Debug.LogError("'IAP_RecieptObject_Android' object json parse error: " + ex1.Message);
                onVerificationComplete?.Invoke(false);
                return;
            }




            foreach (IPurchaseReceipt purchaseReciept in preciepts)
            {
                if (purchaseReciept.productID != product.definition.id) continue;
                else
                {
                    gReceipt = purchaseReciept as GooglePlayReceipt;
                    break;
                }
            }
            if (gReceipt == null)
            {
                Debug.LogError("validated reciept not found!");
                onVerificationComplete?.Invoke(false);
                return;
            }
            else
            {
                processing = true;
                this.onValidationComplete = onVerificationComplete;
                AdjustPurchase.VerifyPurchaseAndroid(product.definition.id, gReceipt.purchaseToken, recieptObject.Payload, VerificationInfoDelegate);
            }
#elif UNITY_IOS
            //AppleInAppPurchaseReceipt iapreciept = null;
            //foreach (IPurchaseReceipt purchaseReciept in preciepts)
            //{
            //    if (purchaseReciept.productID != product.definition.id) continue;
            //    else
            //    {
            //        iapreciept = purchaseReciept as AppleInAppPurchaseReceipt;
            //        break;
            //    }
            //}

            //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            //// Get a reference to IAppleConfiguration during IAP initialization.
            //var appleConfig = builder.Configure<IAppleConfiguration>();
            //var receiptData = Convert.FromBase64String(appleConfig.appReceipt);
            //AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);

            //AppleInAppPurchaseReceipt appleProductReciept = null;
            //foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts)
            //{
            //    if (product.definition.id == productReceipt.productID)
            //    {
            //        appleProductReciept = productReceipt;
            //    }
            //}
            //if (appleProductReciept == null)
            //{

            //}
            if (product.receipt == null || product.transactionID == null)
            {
                Debug.LogError("AdjustPurchase: Invalid purchase parameters.");
                onVerificationComplete?.Invoke(false);
                return;
            }


            processing = true;
            this.onValidationComplete = onVerificationComplete;
            if(enableModuleLogs)string.Format("Adjust purchase verifier called: {0} _ {1} _ {2}", product.definition.id, product.transactionID, product.receipt).Log(LogColorCode) ;
            AdjustPurchase.VerifyPurchaseiOS(product.receipt, product.transactionID, product.definition.id, VerificationInfoDelegate);
#endif


        }

        void VerificationInfoDelegate(ADJPVerificationInfo verificationInfo)
        {
            switch (verificationInfo.VerificationState)
            {
                case ADJPVerificationState.ADJPVerificationStatePassed:
                    if (enableModuleLogs) "purchase validation successful".Log(LogColorCode);
                    onValidationComplete?.Invoke(true);
                    break;
                default:
                    Debug.LogErrorFormat("Purchase validation was not successfull with status {0}", verificationInfo.VerificationState);
                    Debug.Log("Purchase validation message: " + verificationInfo.Message);
                    Debug.Log("Purchase validation status code: " + verificationInfo.StatusCode);
                    onValidationComplete?.Invoke(false);
                    break;
            }
            onValidationComplete = null;
            processing = false;
        }
    }
    [System.Serializable]
    public class IAP_RecieptObject_Android
    {
        public string Store;
        public string TransactionID;
        [Multiline]
        public string Payload;
    }


    [System.Serializable]
    public class IAP_PayloadObject_Android
    {
        [Multiline]
        public string json;
        [Multiline]
        public string signature;
        [Multiline]
        public string skuDetails;
        public bool isPurchaseHistorySupported;
    }
}
#endif
#endif