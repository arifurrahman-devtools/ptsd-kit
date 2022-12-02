using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PTSDKit
{
    public class ControlCenter : MonoBehaviour
    {
        private static string maxKey ="";
        public bool disablePTSDLogs;
        public bool skipUsingSplashScene;
        public bool skipAutoLoadNextScene;
        public int autoLoadSceneIndex=1;
        const string LogColorCode = "00ffff";
        public static bool IsReady { get; private set; }
        public static float progress { get; private set; }
        public static bool initialized { get; private set; }

        public static bool consentDialogApplicableDetected { get; private set; }
        public static bool ios14_5plusDetected { get; private set; }



        public static bool hasConsent { get; private set; }
        static System.Action<bool> onInitializationCallback;
        static System.Action onPTSDIsReady;

        static GameObject ccGo;
               

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitialLoad()
        {
#if !UNITY_IOS

            hasConsent = true;
#else
            hasConsent = false;
#endif
#if PTSDK_MAX
            
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
#if UNITY_IOS
                if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    // Note that App transparency tracking authorization can be checked via `sdkConfiguration.AppTrackingStatus` for Unity Editor and iOS targets
                    // 1. Set Facebook ATE flag here, THEN
                    hasConsent = (sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);
                    ios14_5plusDetected = true;

                }
                else
                {
                    hasConsent = true;
                }
#else
                hasConsent = true;
#endif

                consentDialogApplicableDetected = (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies);
                Debug.Log("<color=#00ffff>Max Initialized</color>");
                OtherInit();
            };
            MaxSdk.SetVerboseLogging(false);
            MaxSdk.SetSdkKey(maxKey);
            MaxSdk.InitializeSdk();
#else
            OtherInit();
#endif
        }

        static ControlCenter cc;

        static void OtherInit()
        {
            if (initialized) return;




            ccGo =  MonoBehaviour.Instantiate( Resources.Load<GameObject>("PTSDKit"));
            cc = ccGo.GetComponent<ControlCenter>();
            GameObject.DontDestroyOnLoad(ccGo);

            foreach (Transform tr in ccGo.transform)
            {
                foreach (var mono in tr.GetComponents<MonoBehaviour>())
                {
                    IEarlyInitiatable ip = mono as IEarlyInitiatable;
                    if (ip != null)
                    {
                        cc.initialCount++;
                        cc.mods.Add(ip);
                        if (cc.disablePTSDLogs) ip.ForceDisableLogs();
                        try
                        {
                            ip.InitializeSuperEarly(hasConsent, ModuleReadyToUse);
                        }
                        catch(Exception e)
                        {
                            Debug.LogError(e.Message);
                            Debug.LogErrorFormat("A PTSD SDK wrapper has failed to initialize properly!: {0}", mono.GetType());
                            if (cc.mods.Contains(ip))
                            {
                                cc.mods.Remove(ip);
                            }
                        }
                    }
                }
            }


            cc.StartCoroutine(cc.WaitForReady());

            onInitializationCallback?.Invoke(hasConsent);
            initialized = true;

        }

        static void ModuleReadyToUse(IEarlyInitiatable readyPot)
        {
            if (cc.mods.Contains(readyPot))
            {
                cc.mods.Remove(readyPot);
            }
            else Debug.LogError("Ready PTSDItem is not listed!");
        }

        float initialCount;
        List<IEarlyInitiatable> mods = new List<IEarlyInitiatable>();
        IEnumerator WaitForReady()
        {
            if (!cc.disablePTSDLogs) ("PTSD Created").Log(LogColorCode);
            float lastReadiness=-1;
            while (mods.Count>0)
            {
                progress = 1 - (mods.Count / initialCount);
                if (progress != lastReadiness)
                {
                    if (!cc.disablePTSDLogs)
                    {
                        if (PTSDSplash.Instance)
                        {
                            PTSDSplash.Instance.SetProgress(progress);
                        }
                        else
                        {
                            string.Format("PTSD readiness: {0}", progress).Log(LogColorCode);
                        }
                    }
                }
                yield return new WaitForSeconds(0.25f);
                lastReadiness = progress;
            }
            if (!skipUsingSplashScene)
            {
                while (PTSDSplash.Instance == null)
                {
                    yield return null;
                }
                PTSDSplash.Instance.SetProgress(1);
                yield return new WaitForSeconds(0.5f);
                if (!skipAutoLoadNextScene)
                {
                    IsReady = true;
                    onPTSDIsReady?.Invoke();
                    if (autoLoadSceneIndex > 0)
                    {
                        SceneManager.LoadScene(autoLoadSceneIndex);
                    }
                    else
                    {
                        SceneManager.LoadScene(1);
                    }
                }
                else
                {
                    IsReady = true;
                    onPTSDIsReady?.Invoke();
                }
            }
            else 
            {
                IsReady = true;
                onPTSDIsReady?.Invoke();
            }


            if (!cc.disablePTSDLogs) ("PTSD Is Ready").Log(LogColorCode);
        }


        public static void ExecuteOnInitConfirmed(System.Action<bool> onConfirmed)
        {
            if (initialized)
            {
                onConfirmed?.Invoke(hasConsent);
            }
            else
            {
                onInitializationCallback += onConfirmed;
            }
        }

        public static void ExecuteWhenPTSDReady(System.Action onReady)
        {
            if (IsReady)
            {
                onReady?.Invoke();
            }
            else
            {
                onPTSDIsReady += onReady;
            }
        }
    }

    public interface IEarlyInitiatable
    {
        void InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse);
        string LogColorCode { get; }
        bool IsReady { get; set; }
        void ForceDisableLogs();

    }

    public enum LogLevel
    {
        None = 0,
        Critical = 1,
        Important = 2,
        All = 3,
    }
}

