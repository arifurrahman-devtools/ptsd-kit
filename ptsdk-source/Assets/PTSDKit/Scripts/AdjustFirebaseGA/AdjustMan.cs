using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PTSDK_ADJUST
using com.adjust.sdk;
#endif


namespace PTSDKit
{
    public class AdjustMan : MonoBehaviour, IEarlyInitiatable
    {
        public IDKeeper appToken;

        bool enableModuleLogs = true;
        public string LogColorCode => "4B5F81";
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }
#if !PTSDK_ADJUST
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        private const string errorMsgEditor = "[Adjust]: SDK can not be used in Editor.";
        private const string errorMsgStart = "[Adjust]: SDK not started. Start it manually using the 'start' method.";
        private const string errorMsgPlatform = "[Adjust]: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.";
        public static AdjustMan Instance { get; private set; }


        public static bool isTestMode
        {
            get
            {
                return string.Equals(Application.identifier, testbundleid);

            }
        }
        private const string androidTestKey="";
        private const string iosTestKey="";
        private const string androidTestEventKey = "";
        private const string iosTestEventKey = "";
        public const string testbundleid = "";
        private IDKeeper _appTokenTest;
        private IDKeeper appTokenTest
        {
            get
            {
                if (_appTokenTest == null)
                {
                    _appTokenTest = new IDKeeper(androidTestKey, iosTestKey);
                }
                return _appTokenTest;
            }
        }
       
        public string activeAppToken
        {
            get
            {
                return isTestMode ? appTokenTest.id : appToken.id;
            }
        }

       
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            AdjustConfig adjustConfig = new AdjustConfig(activeAppToken, AdjustEnvironment.Production, false);
            adjustConfig.setLogLevel(AdjustLogLevel.Info);
            adjustConfig.setSendInBackground(false);
            adjustConfig.setEventBufferingEnabled(false);
            adjustConfig.setLaunchDeferredDeeplink(true);
            adjustConfig.setDelayStart(0);

            Adjust.start(adjustConfig);

            if (isTestMode)
            {
                IDKeeper testEventID = new IDKeeper(androidID: androidTestEventKey, iosID: iosTestEventKey);
                AdjustEvent testAdjustEvent = new AdjustEvent(testEventID.id);
                Adjust.trackEvent(testAdjustEvent);
            }
            Instance = this; 
            
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
        }
        void OnApplicationPause(bool pauseStatus)
        {
            if (!IsReady) return;

#if UNITY_IOS && !UNITY_EDITOR
            return;
#endif
            if (IsEditor())
            {
                return;
            }

#if UNITY_ANDROID
            if (pauseStatus)
            {
                AdjustAndroid.OnPause();
            }
            else
            {
                AdjustAndroid.OnResume();
            }
#else
            if (enableModuleLogs) errorMsgPlatform.Log(LogColorCode);
#endif
        }
        private  bool IsEditor()
        {
#if UNITY_EDITOR
            if (enableModuleLogs) errorMsgPlatform.Log(errorMsgEditor);
            return true;
#else
            return false;
#endif

        }


#endif
        }
}
