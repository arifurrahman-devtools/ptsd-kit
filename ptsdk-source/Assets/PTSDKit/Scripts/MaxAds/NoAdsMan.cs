using System;
using System.Collections;
using UnityEngine;
namespace PTSDKit
{
    public class NoAdsMan : MonoBehaviour, IEarlyInitiatable
    {
        public static bool isBlockingAds = false;
        bool enableModuleLogs;
        string IEarlyInitiatable.LogColorCode => "ffffff";

        public bool IsReady { get; set; }

        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }


#if PTSDK_NOAD && PTSDK_IAP 
        public PurchaseType productIdentifier;
        NonConsumablePTSDItem noadProduct;
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            StartCoroutine(WaitForInit());
            onModuleReadyToUse?.Invoke(this);
        }
        IEnumerator WaitForInit()
        {
            while (!InAppPurchaseMan.Instance)
            {
                yield return null;
            }
            noadProduct = InAppPurchaseMan.GetProduct(productIdentifier) as NonConsumablePTSDItem;
            if (noadProduct == null)
            {
                Debug.LogError("no ad product not defined or is not NonConsumable");
            }
            else
            {
                noadProduct.OnItemAvailable ( () =>
                {
                    isBlockingAds = true;
                });
            }
        }

#else
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#endif
    }
}