using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PTSDK_FIREBASE
using Firebase;
#endif

namespace PTSDKit
{
    public class FireBaseMan : MonoBehaviour,IEarlyInitiatable
    {
        public static FireBaseMan Instance { get; private set; }
        bool enableModuleLogs = true;
        public string LogColorCode => "f5820d";
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }

#if !PTSDK_FIREBASE
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        

        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
            Instance = this;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    FirebaseApp firebaseApp = FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                }
                else
                {
                    Debug.LogError(string.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }



        public bool enableTestAnalytics = false;
        IEnumerator Start()
        {
            int i = 0;
            yield return null;
            while (enableTestAnalytics)
            {
                if (IsReady)
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("testevent", "timepassed", i);
                    i += 2;
                    if (enableModuleLogs) "FireBase log sent".Log(LogColorCode);
                    yield return new WaitForSeconds(2);
                }
                else yield return null;

            }
        }
#endif
    }
}

