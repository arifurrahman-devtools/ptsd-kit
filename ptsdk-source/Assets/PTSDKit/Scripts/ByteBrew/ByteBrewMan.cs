using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PTSDK_BYTEBREW
using ByteBrewSDK;
#endif
namespace PTSDKit
{
    public class ByteBrewMan : MonoBehaviour, IEarlyInitiatable
    {
        public static ByteBrewMan Instance { get; private set; }
        bool enableModuleLogs = true;
        public string LogColorCode => "2255FF";
        public bool IsReady { get; set; }

        public GameObject prefabReference;

        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
#if PTSDK_BYTEBREW
            Instance = this;
            Instantiate(prefabReference);

            Centralizer.Add_DelayedAct(() => {
                ByteBrew.InitializeByteBrew();
                onModuleReadyToUse?.Invoke(this);
            }, 0.1f);
#else
            onModuleReadyToUse?.Invoke(this);
#endif
        }
    }


}