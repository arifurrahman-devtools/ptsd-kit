using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
#if !PTSDK_APPSFLYER
namespace PTSDKit
{
    public class AppsFlyerMan : MonoBehaviour, IEarlyInitiatable
    {
        public static AppsFlyerMan Instance { get; private set; }

        public string LogColorCode => "FFFF00";

        public bool IsReady { get; set; }

        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }


        void IEarlyInitiatable.ForceDisableLogs()
        {
        }
    }

}

#else

using AppsFlyerSDK;

namespace PTSDKit
{
    public class AppsFlyerMan : MonoBehaviour, IEarlyInitiatable, IAppsFlyerConversionData
    {
        public static AppsFlyerMan Instance { get; private set; }

        public string LogColorCode => "FFFF00";

        public bool IsReady { get; set; }
        //public string devKey;
        public string ios_appID;
        private bool isDebug = false;
        private bool getConversionData = true;

        Action<IEarlyInitiatable> onReady;
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onReady = onModuleReadyToUse;
        }
        void Start()
        {
            Instance = this;

            AppsFlyer.setIsDebug(isDebug);
            AppsFlyer.initSDK("nYwfftoacbopmuszWBPGnd", ios_appID, getConversionData ? this : null);
            AppsFlyer.startSDK();


            IsReady = true;
            onReady?.Invoke(this);
        }

        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here

            AppsFlyer.sendEvent("af_integration_success", new Dictionary<string, string>());
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }


        void IEarlyInitiatable.ForceDisableLogs()
        {
        }
    }

}

#endif
