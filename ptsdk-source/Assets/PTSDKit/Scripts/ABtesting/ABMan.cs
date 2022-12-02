using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PTSDKit
{
    public class ABMan : MonoBehaviour, IEarlyInitiatable
    {
        public LogLevel loglevel = LogLevel.Important;
        public string LogColorCode => "22AA22";
        public bool IsReady { get; set; }
        void IEarlyInitiatable.ForceDisableLogs()
        {
            loglevel = LogLevel.None;
        }




        public static ABMan Instance { get; private set; }
        private Dictionary<ABtype, HardAB> _allSettings;
        public Dictionary<ABtype, HardAB> allSettings
        {
            get
            {
                if (_allSettings == null) Init();
                return _allSettings;
            }
        }
        private void Init()
        {
            Instance = this;
            HardAB.logColorCode = LogColorCode;
            HardAB.ActiveLogLevel = loglevel;
#if PTSDK_MAX
            _allSettings = new Dictionary<ABtype, HardAB>();

            foreach (var entry in AB_entries)
            {
                HardAB hardAB=null;
#if UNITY_IOS

                if (!entry.disable_iOS) hardAB = new HardAB(entry.ab_type, entry.title, entry.editorDefaultValue, entry.isVolatile);
#elif UNITY_ANDROID
                if (!entry.disable_Android) hardAB = new HardAB(entry.ab_type, entry.title, entry.editorDefaultValue, entry.isVolatile);
#endif
                if (hardAB != null)
                {
                    entry.hardAB = hardAB;
                    allSettings.Add(entry.ab_type, hardAB);
                    hardAB.Assign_IfUnassigned(MaxSdk.VariableService.GetString(hardAB.GetKey()), (ABtype type, int v) => 
                    {
                        if(!enableABTestPanel && customDefinitionSetup.type == type) customDefinitionSetup.CheckCustomDimensions(v);
                    });
                }
            }
#endif
        }

        public static HardAB GetABAccess(ABtype type)
        {
            if (!Instance) return null;
            return Instance.allSettings[type];
        }
        public GameObject prefabReference;
        public bool enableABTestPanel = false;//=> BuildSpecManager.enableABTestUI;
        public List<ABKeep> AB_entries;
        public CustomDefSetup customDefinitionSetup;
        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IEarlyInitiatable> onModuleReadyToUse)
        {
#if PTSDK_AB_TEST && PTSDK_MAX
            Init();
            if (enableABTestPanel)
            {

                GameObject go = Instantiate(prefabReference);
                go.transform.SetParent(this.transform);
                go.GetComponent<TestABController>().Init(()=> 
                {
                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                }, AB_entries);

            }
            else
            {
                onModuleReadyToUse?.Invoke(this);
                IsReady = true;
            }
#else
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
#endif
        }

        public static int GetValue_Int(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            int value;
            GetABAccess(type).GetValue(out value);
            return value;
        }
        public static float GetValue_Float(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            float value;
            GetABAccess(type).GetValue_float(out value);
            return value;
        }
        public static bool GetValue_Bool(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            bool value;
            GetABAccess(type).GetValue(out value);
            return value;
        }
        public static string GetValue_String(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            string value;
            GetABAccess(type).GetValue(out value);
            return value;
        }
    }
    [System.Serializable]
    public class ABKeep
    {
        public string title;
        public ABtype ab_type;
        public bool disable_iOS;
        public bool disable_Android;
        public bool isVolatile;
        public string editorDefaultValue;
        public List<ABDisplayDefinitions> displayDefinitions;

        [System.NonSerialized] public HardAB hardAB;


        
    }

    [System.Serializable]
    public class ABDisplayDefinitions
    {
        public string value;
        public string significance;
    }
    [System.Serializable]
    public class CustomDefinitionPairs
    {
        public int value;
        public string customDefinition;
    }
    [System.Serializable]
    public class CustomDefSetup
    {
        public ABtype type;
        public List<CustomDefinitionPairs> definitions;

        static HardData<bool> reportedCustomDimensions;
        public void CheckCustomDimensions(int value)
        {
            if (definitions.Count == 0) return;
            if (reportedCustomDimensions == null)
            {
                reportedCustomDimensions = new HardData<bool>("CUSTOM_DIM_SET",false);
            }

#if PTSDK_GAME_ANALYTICS
            if (reportedCustomDimensions)
            {
                return;
            }

            string dim_string = "";

            for (int i = 0; i < definitions.Count; i++)
            {
                if (definitions[i].value == value)
                {
                    dim_string = definitions[i].customDefinition;
                    break;
                }
            }
            //$"found dim string: {dim_string}".Log("FFFF00");

            if (string.IsNullOrEmpty(dim_string))
            {
                Debug.LogError("custom dimension setup is faulty");
                return;
            }
            if (GameAnalyticsMan.Instance && GameAnalyticsMan.Instance.IsReady)
            {
                GameAnalyticsSDK.GameAnalytics.SetCustomDimension01(dim_string);
                $"Custom Dimension: {dim_string}".Log("FF9900");
                reportedCustomDimensions.value = true;
            }
            else
            {
                GameAnalyticsMan.onAnalyticsReady += () =>
                {
                    GameAnalyticsSDK.GameAnalytics.SetCustomDimension01(dim_string);
                    $"Custom Dimension: {dim_string}".Log("FF9900");
                    reportedCustomDimensions.value = true;
                };
            }
#endif
        }
    }
}
