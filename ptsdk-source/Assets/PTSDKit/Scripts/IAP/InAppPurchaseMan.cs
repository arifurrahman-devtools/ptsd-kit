using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PTSDK_IAP
using UnityEngine.Purchasing;
#if PTSDK_IAP_VALIDATION
using com.adjust.sdk;
#endif
#endif

namespace PTSDKit
{
#if !PTSDK_IAP

    public class InAppPurchaseMan : MonoBehaviour
    {
        public GameObject helperCanvasPrefab;
#else
    public class InAppPurchaseMan : MonoBehaviour, IStoreListener, IEarlyInitiatable
    {
#if UNITY_EDITOR
        public string LogColorCode => "EEB609";
#else

        public string LogColorCode => "B78628";
#endif
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            ActiveLogLevel = LogLevel.None;
        }

        public static InAppPurchaseMan Instance { get; private set; }
        public static Dictionary<string, PurchaseablePTSDItem> allPurchasables = new Dictionary<string, PurchaseablePTSDItem>();
        public static GameObject shield;

        public LogLevel ActiveLogLevel = LogLevel.Critical;
        public bool enableTestUI = true;

        public List<NonConsumablePTSDItem> nonConsumablePurchases = new List<NonConsumablePTSDItem>();

        public List<ConsumablePTSDItem> consumablePurchases = new List<ConsumablePTSDItem>();
        [Space]
        [Space]
        public GameObject helperCanvasPrefab;
        
        
        IAPHelperCanvasController helperCanvas;
        IExtensionProvider extension;


        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extension)
        {
            this.extension = extension;
            foreach (var item in allPurchasables) item.Value.OnStoreInit(controller);
            if ((int)ActiveLogLevel >= (int) LogLevel.Important) "iap initialized".Log(LogColorCode);
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("initialization failed");
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            shield.SetActive(false);
            string id = product.definition.id;
            allPurchasables[id].ReportPurchaseResult(false);
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            string id = purchaseEvent.purchasedProduct.definition.id;
#if PTSDK_IAP_VALIDATION
            AdjustIAPVerifyMan.VerifyPurchase(purchaseEvent.purchasedProduct, (bool onValidationPassed) =>
            {
                ExecutePurchaseAction(purchaseEvent.purchasedProduct, allPurchasables[id], onValidationPassed);
            });
#else
            ExecutePurchaseAction(purchaseEvent.purchasedProduct,allPurchasables[id],true);
#endif
            return PurchaseProcessingResult.Complete;
        }
        void ExecutePurchaseAction(Product product, PurchaseablePTSDItem purchaseDefinition, bool hasValidationPassedorNotRequired)
        {
            shield.SetActive(false);
            string id = product.definition.id;
            purchaseDefinition.ReportPurchaseResult(hasValidationPassedorNotRequired);
            if ((int)ActiveLogLevel >= (int)LogLevel.Important && hasValidationPassedorNotRequired) string.Format( "validated(or no validation required) purchase for id: {0}", id).Log(LogColorCode); ;
            if ((int)ActiveLogLevel >= (int)LogLevel.Critical && !hasValidationPassedorNotRequired) string.Format("validation failed for id: {0}", id).Log(LogColorCode); ;

#if PTSDK_IAP_VALIDATION
            if (purchaseDefinition.adjustEventToken.isValid && hasValidationPassedorNotRequired)
            {
                try
                {
                    AdjustEvent adjustEvent = new AdjustEvent(purchaseDefinition.adjustEventToken);
                    adjustEvent.setRevenue(decimal.ToDouble(product.metadata.localizedPrice), product.metadata.isoCurrencyCode);
                    adjustEvent.setTransactionId(JsonUtility.FromJson<IAP_RecieptObject_Android>(product.receipt).TransactionID);
                    Adjust.trackEvent(adjustEvent);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to send adjust purchase log. Error Message: " + e.Message);
                }
            }
#endif

        }

        System.Action<IEarlyInitiatable> onModuleReadyToUse;
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            this.onModuleReadyToUse = onModuleReadyToUse;
            foreach (var item in nonConsumablePurchases) allPurchasables.Add(item.productID, item);
            foreach (var item in consumablePurchases) allPurchasables.Add(item.productID, item);
            PurchaseablePTSDItem.ActiveLogLevel = ActiveLogLevel;
            PurchaseablePTSDItem.LogColorCode = LogColorCode;

            StandardPurchasingModule purchasingModule = StandardPurchasingModule.Instance();
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(purchasingModule);
            foreach (var item in allPurchasables) item.Value.ItemInit(builder);
            UnityPurchasing.Initialize(this, builder);


            helperCanvas = Instantiate(helperCanvasPrefab).GetComponent<IAPHelperCanvasController>();
            helperCanvas.transform.SetParent(this.transform);
            helperCanvas.testCanvas.SetActive(enableTestUI);
            shield = helperCanvas.protectionShield;
            shield.SetActive(false);

            if (helperCanvas)
            {
                helperCanvas.restorePurchaseIOS.onClick.AddListener(() =>
                {
#if UNITY_IOS
                    RequestRestorePurchase_IOS();
#else
                    if ((int)ActiveLogLevel >= (int)LogLevel.Important) "This only works in IOS device. Android dont need manual restore!".Log(LogColorCode);
#endif


                });
                helperCanvas.no_ad.onClick.AddListener(TestNoad);
                helperCanvas.diva_pack.onClick.AddListener(TestDiva);
                helperCanvas.coin_small.onClick.AddListener(TestPack1);
                helperCanvas.coin_large.onClick.AddListener(TestPack2);
            }
            Instance = this;
        }



        public static void RestorePurchase_IOS()
        {
            Instance.RequestRestorePurchase_IOS();
        }
        public void RequestRestorePurchase_IOS()
        {
#if UNITY_IOS
            IAppleExtensions appleStore = extension.GetExtension<IAppleExtensions>();
            
            if ((int)ActiveLogLevel >= (int) LogLevel.All) string.Format("restore request- apple store ref: {0}", appleStore).Log(LogColorCode);
            appleStore.RestoreTransactions(result => {
                if (result)
                {
                    if ((int)ActiveLogLevel >= (int) LogLevel.All) "Purchases should be restored if applicable".Log(LogColorCode);
                }
                else
                {
                    Debug.LogError("restoration failed");
                }
            });
#else
            Debug.LogError("You dont need to call restore purchase manually on non-ios devices");
#endif
        }



        public static PurchaseablePTSDItem GetProduct(PurchaseType type)
        {
            foreach (var item in allPurchasables)
            {
                if (item.Value.typeID == (int) type)
                {
                    return item.Value;
                }
            }
            Debug.LogError("Product Not Found");
            return null;
        }

        public void TestPurchase(PurchaseType type)
        {
            PurchaseablePTSDItem product = GetProduct(type);
            if(product!=null) product.RequestPurchase((bool success) => 
            {
                if ((int)ActiveLogLevel >= (int)LogLevel.Critical) string.Format("{0} purchase success: {1}", product.title, success).Log(LogColorCode); 
            });
        }
        public void TestRestorePurchase()
        {
            RequestRestorePurchase_IOS();
        }

        public void TestNoad()
        {

            TestPurchase((PurchaseType)0);
        }
        public void TestDiva()
        {
            TestPurchase((PurchaseType)1);
        }
        public void TestPack1()
        {
            TestPurchase((PurchaseType)(0 + nonConsumablePurchases.Count));
        }
        public void TestPack2()
        {
            TestPurchase((PurchaseType)(1 + nonConsumablePurchases.Count));
        }



#endif
        }
}


