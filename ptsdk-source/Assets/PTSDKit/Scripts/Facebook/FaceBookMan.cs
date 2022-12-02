
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PTSDK_FACEBOOK
using Facebook.Unity;
#endif
namespace PTSDKit
{
    public class FaceBookMan : MonoBehaviour, IEarlyInitiatable
    {
        bool enableModuleLogs = true;
        public string LogColorCode => "29487d";
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }

#if !PTSDK_FACEBOOK
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        //public bool enableTestAnalytics = false;

        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            if (FB.IsInitialized)
            {
                OnFBInitializationConfirmed(hasConsent);

                onModuleReadyToUse?.Invoke(this);
                IsReady = true;
            }
            else
            {

                FB.Init(() => {
                    OnFBInitializationConfirmed(hasConsent);

                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                });
            }
        }
        void OnFBInitializationConfirmed(bool hasConsent)
        {
            FB.ActivateApp();
#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if PTSDK_MAX
                FB.Mobile.SetAdvertiserTrackingEnabled(hasConsent);
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(advertiserTrackingEnabled: hasConsent);
#endif
            }
            if (enableModuleLogs) string.Format("Consent={0}, will be sent if run from iOS device",hasConsent).Log(LogColorCode);
#endif
        }

#endif
    }
}

