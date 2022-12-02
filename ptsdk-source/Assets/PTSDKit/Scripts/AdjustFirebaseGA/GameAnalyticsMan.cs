using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PTSDK_GAME_ANALYTICS
using GameAnalyticsSDK;
#endif

namespace PTSDKit
{
    public class GameAnalyticsMan : MonoBehaviour, IEarlyInitiatable
    {
        public static GameAnalyticsMan Instance { get; private set; }
        public static event System.Action onAnalyticsReady;

        bool enableModuleLogs = true;
        public string LogColorCode => "8CD14F";
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }
#if !PTSDK_GAME_ANALYTICS
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else

        public GameObject prefabReference;


        public bool enableTestAnalytics = false;

        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            Instance = this;
            Instantiate(prefabReference);

            Centralizer.Add_DelayedAct(() => {
                GACore.enableModuleLogs = enableModuleLogs;
                GACore.LogColorCode = LogColorCode;
                GACore.Init(() =>
                {
                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                    onAnalyticsReady?.Invoke();
                });
            }, 0.1f);

        }



        IEnumerator Start()
        {
            int i = 0;
            yield return null;
            while (enableTestAnalytics)
            {
                if (IsReady)
                {
                    GameAnalytics.NewDesignEvent(string.Format("testevent:timepassed:{0}", i));
                    i += 2;
#if !UNITY_EDITOR
                    if (enableModuleLogs) "GA log sent".Log(LogColorCode);
#endif
                    yield return new WaitForSeconds(2);
                }
                else yield return null;

            }
        }
#endif

    }

#if PTSDK_GAME_ANALYTICS
    public class GACore : IGameAnalyticsATTListener
    {
        static GACore Instance { get; set; }

        private System.Action onInit;

        bool initialized { get; set; }

        void IGameAnalyticsATTListener.GameAnalyticsATTListenerAuthorized()
        {
            ManualInit();
            if (enableModuleLogs) "GA log sent".Log(LogColorCode);
        }

        void IGameAnalyticsATTListener.GameAnalyticsATTListenerDenied()
        {

            ManualInit();
            Debug.LogError("GA : ATTListenerDenied");
        }

        void IGameAnalyticsATTListener.GameAnalyticsATTListenerNotDetermined()
        {

            ManualInit();
            Debug.LogError("GA : ATTListenerNotDetermined");
        }

        void IGameAnalyticsATTListener.GameAnalyticsATTListenerRestricted()
        {
            ManualInit();
            Debug.LogWarning("GA : ATTListenerRestricted");
        }

        void ManualInit()
        {
            GameAnalytics.Initialize();
            onInit?.Invoke();
            initialized = true;
        }
        GACore()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer && ControlCenter.ios14_5plusDetected)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                ManualInit();
            }
        }
        public static bool enableModuleLogs;
        public static string LogColorCode;

        public static void  Init(System.Action onInitialized)
        {

            if (Instance == null)
            {
                Instance = new GACore();
            }

            if (Instance.initialized)
            {
                onInitialized?.Invoke();
            }
            else
            {
                Instance.onInit += onInitialized;
            }
        }

    }
#endif
}
