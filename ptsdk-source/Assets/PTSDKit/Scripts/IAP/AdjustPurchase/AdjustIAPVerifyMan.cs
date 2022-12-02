using System;
using System.Collections;
using UnityEngine;

#if PTSDK_IAP
using UnityEngine.Purchasing;
#if PTSDK_IAP_VALIDATION
using com.adjust.sdk.purchase;
#endif
#endif


namespace PTSDKit
{
    public class AdjustIAPVerifyMan : MonoBehaviour, IEarlyInitiatable
    {
        bool enableModuleLogs = true;
        public string LogColorCode => "b78628";
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }
#if !PTSDK_IAP_VALIDATION || !PTSDK_IAP
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        [SerializeField] bool verifyConsumable = false;
        [SerializeField] bool verifyNonConsumable = true;
        bool verifySubscription = false;
        public static AdjustIAPVerifyMan Instance { get; private set; }

        System.Action<IEarlyInitiatable> onModuleReadyToUse;
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            this.onModuleReadyToUse = onModuleReadyToUse;
            VerificationRequest.enableModuleLogs = enableModuleLogs;
            VerificationRequest.LogColorCode = LogColorCode;
            StartCoroutine(WaitForAdjust());
        }
        IEnumerator WaitForAdjust()
        {
            while (!AdjustMan.Instance)
            {
                yield return null;
            }
            if (enableModuleLogs) "Adjust purchase verifier initiated".Log(LogColorCode);
            ADJPConfig config = new ADJPConfig(AdjustMan.Instance.appToken, ADJPEnvironment.Production);
            config.SetLogLevel(ADJPLogLevel.Error);
            AdjustPurchase.Init(config);
            Instance = this;
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
        }

        public static void VerifyPurchase(Product product, Action<bool> onVerificationPassedOrNotRequired)
        {
            if (Instance == null)
            {
                Debug.LogError("Adjust Purchase not initialized!");
                onVerificationPassedOrNotRequired?.Invoke(false);
                return;
            }
            switch (product.definition.type)
            {
                case ProductType.Consumable:
                    if (!Instance.verifyConsumable)
                    {
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
                    break;
                case ProductType.NonConsumable:
                    if(!Instance.verifyNonConsumable)
                    {
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
                    break;
                case ProductType.Subscription:
                    if (!Instance.verifySubscription)
                    {
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
                    break;
                default:
                    {
                        Debug.LogError("Unknown situation!");
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
            }
            new VerificationRequest(product, onVerificationPassedOrNotRequired);
        }
#endif

    }
}