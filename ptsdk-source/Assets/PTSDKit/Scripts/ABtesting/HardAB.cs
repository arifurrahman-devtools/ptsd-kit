using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PTSDK_GAME_ANALYTICS
using GameAnalyticsSDK;
#endif
//using FRIA;

namespace PTSDKit
{
    public class HardAB
    {
        public static string logColorCode = "3a1b13";
        public static LogLevel ActiveLogLevel = LogLevel.Critical;
        ABtype type;
        string key;
        string data_key;
        HardData<string> data;
        bool isVolatile;
        /// <summary>
        /// initiate AB entity
        /// </summary>
        /// <param name="type">type identifier</param>
        /// <param name="key">key to access values from server</param>
        /// <param name="editorDefaultValue">starting value for editor, can be useful in testing</param>
        /// <param name="isVolatile">value is not permanent, can be overwritten by server, set to false for standard AB implementations</param>
        public HardAB(ABtype type, string key, string editorDefaultValue = "", bool isVolatile = false)
        {
            this.isVolatile = isVolatile;
            this.type = type;
            this.key = key;
            data_key = string.Format("AB_KEY_{0}", key);
#if UNITY_EDITOR
            data = new HardData<string>(data_key, editorDefaultValue);
#else
            data = new HardData<string>(data_key, "");
#endif
        }

        public string GetKey()
        {
            return key;
        }
        public void Assign_IfUnassigned(string abStringValue, System.Action<ABtype,int> onFreshValuesSet)
        {

            if (data.value == "" || isVolatile)
            {

                Log(LogLevel.Important, string.Format("[AB test] {0} key value set: {1} from {2}", key, abStringValue, data.value));
                data.value = abStringValue;
                int v;
                GetValue(out v);
                //FRIA.Centralizer.Init();
                onFreshValuesSet?.Invoke(type,v);
               
                
                
                //FRIA.Centralizer.Add_DelayedMonoAct(Centralizer.Instance, () => {
                //    //if (type == ABtype.AB0_rpi_test)
                //    //{
                //    //    int v;
                //    //    GetValue(out v);
                //    //    switch (v)
                //    //    {
                //    //        case 0:
                //    //            GameAnalytics.SetCustomDimension01("fullRV_inter_Energy");
                //    //            break;
                //    //        case 1:
                //    //            GameAnalytics.SetCustomDimension01("fullRV_inter_noEnergy");
                //    //            break;
                //    //        case 2:
                //    //            GameAnalytics.SetCustomDimension01("fullRV_noInter_Energy");
                //    //            break;
                //    //    }
                //    //}
                //    //if (type == ABtype.AB1_rpi_test2)
                //    //{
                //    //    int v;
                //    //    GetValue(out v);
                //    //    switch (v)
                //    //    {
                //    //        case 0:
                //    //            GameAnalytics.SetCustomDimension01("fullRV_inter_Energy");
                //    //            break;
                //    //        case 1:
                //    //            GameAnalytics.SetCustomDimension01("fullRV_inter_noEnergy");
                //    //            break;
                //    //    }
                //    //}
                //    //if (type == ABtype.AB2_simpleUI_inter_test)
                //    //{
                //    //    int v;
                //    //    GetValue(out v);
                //    //    switch (v)
                //    //    {
                //    //        case 0:
                //    //            GameAnalytics.SetCustomDimension01("simple_no_inter");
                //    //            break;
                //    //        case 1:
                //    //            GameAnalytics.SetCustomDimension01("simple_inter");
                //    //            break;
                //    //    }
                //    //}
                //    if (type == ABtype.AB3_level_fixes)
                //    {
                //        int v;
                //        GetValue(out v);
                //        switch (v)
                //        {
                //            case 0:
                //                GameAnalytics.SetCustomDimension01("original_levels");
                //                "original_levels".Debug("FF9900");
                //                break;
                //            case 1:
                //                GameAnalytics.SetCustomDimension01("fixed_levels");
                //                "fixed_levels".Debug("FF9900");
                //                break;
                //        }
                //    }
                //}, 5);
                
            }
            else
            {
                Log(LogLevel.All, string.Format("[AB test] {0} id has retrieved value: {1}", key, abStringValue));
            }
        }

        public void ForceSetValue(string newValue)
        {
            data.value = newValue;

        }
        #region Data Get
        public void GetValue(out string stringValue)
        {
            stringValue = data.value;
            if (data.value == "")
            {
                if (!Application.isEditor)
                {
                    Log(LogLevel.Important, string.Format("[AB test] {0} key's value has returned empty string", key));
                }
            }
        }

        public void GetValue(out bool boolValue)
        {
            bool success = bool.TryParse(data.value, out boolValue);
            if (success)
            {
                return;
            }
            else if (data.value == "")
            {
                boolValue = false;
                data.value = boolValue.ToString();
                Log(LogLevel.Critical, string.Format("[AB test] {0} key value was defaulted to {1}", key, data.value));

            }
            else
            {
                boolValue = false;
                UnityEngine.Debug.LogErrorFormat("{0} key was miscast as boolean! using {1} as result value...", key, boolValue);
            }
        }

        public void GetValue(out int numberValue)
        {
            bool success = int.TryParse(data.value, out numberValue);
            if (success)
            {
                return;
            }
            else if (data.value == "")
            {
                numberValue = 0;
                data.value = numberValue.ToString();
                Log(LogLevel.Critical, string.Format("[AB test] {0} key value was defaulted to {1}", key, data.value));

            }
            else
            {
                numberValue = 0;
                UnityEngine.Debug.LogErrorFormat("{0} key was miscast as integer! using {1} as result value...", key, numberValue);
            }
        }

        public void GetValue_float(out float floatValue)
        {
            bool success = float.TryParse(data.value, out floatValue);
            if (success)
            {
                return;
            }
            else if (data.value == "")
            {
                floatValue = 0;
                data.value = floatValue.ToString();
                Log(LogLevel.Critical, string.Format("[AB test] {0} key value was defaulted to {1}", key, data.value));

            }
            else
            {
                floatValue = 0;
                UnityEngine.Debug.LogErrorFormat("{0} key was miscast as float! using {1} as result value...", key, floatValue);
            }
        }

        #endregion
        void Log(LogLevel requiredLogLevel, string str)
        {
            if ((int)ActiveLogLevel >= (int)requiredLogLevel) str.Log(logColorCode);
        }
    }

}