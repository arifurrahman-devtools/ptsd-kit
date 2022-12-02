using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PTSDKit
{
    public static class HardDataCleaner
    {
        public static void Clean()
        {
            HardData<int>.ResetAllHardData();
            HardData<float>.ResetAllHardData();
            HardData<string>.ResetAllHardData();
            HardData<bool>.ResetAllHardData();
        }
    }
    public class HardData<T>
    {

        public static implicit operator T (HardData<T> hd) => hd.value;

        string savedKey;
        T localValue;
        SettingTypes type;

        public static List<string> keyList = new List<string>();
        public static void ResetAllHardData()
        {
            foreach (string key in keyList)
            {
                PlayerPrefs.HasKey(key);
                PlayerPrefs.DeleteKey(key);
            }
        }

        public HardData(string Key, T initValue){
            #region defineType
            if (typeof(T) == typeof(bool))
            {
                type = SettingTypes._bool;
            }
            else if (typeof(T) == typeof(int)) 
            {
                type = SettingTypes._int;
            }
            else if (typeof(T) == typeof(float)) 
            {
                type = SettingTypes._float;
            }
            else if (typeof(T) == typeof(string)) 
            {
                type = SettingTypes._string;
            }
            else if ( typeof ( T ) == typeof ( DateTime ) )
            {
                type = SettingTypes._dateTime;
            }
            else 
            {
                type = SettingTypes._UNDEFINEDTYPE;
                Debug.LogError ("Undefined setting type!!!");
            }
            #endregion
            savedKey = Key;
            if (keyList.Contains(savedKey))
                Debug.LogWarningFormat("Duplicate keys: {0}!",savedKey);
            keyList.Add(savedKey);
            localValue = initValue;
            loadFromPref ();
            saveToPref();
        }

        public T value
        {
            set
            { 
                localValue = value;
                saveToPref ();
            }
            get
            {
                return localValue;
            }
        }
        public string GetKey()
        {
            return savedKey;
        }

        void saveToPref()
        {
            switch (type) {
                default:
                    Debug.LogError ("Pref saving not defined for this type");
                    break;
                case SettingTypes._bool:
                    {
                        bool locBoolValue = (bool)Convert.ChangeType (localValue, typeof(bool));
                        int prefValue = (locBoolValue ? 1 : 0);
                        PlayerPrefs.SetInt (savedKey, prefValue);
                    }
                    break;
                case SettingTypes._int:
                    {
                        int locValue = (int)Convert.ChangeType (localValue, typeof(int));
                        PlayerPrefs.SetInt (savedKey, locValue);
                    }
                    break;      
                case SettingTypes._float:
                    {
                        float locValue = (float)Convert.ChangeType (localValue, typeof(float));
                        PlayerPrefs.SetFloat (savedKey, locValue);
                    }
                    break;
                case SettingTypes._string:
                    {
                        string locValue = (string)Convert.ChangeType (localValue, typeof(string));
                        PlayerPrefs.SetString (savedKey, locValue);
                    }
                    break;
                case SettingTypes._dateTime:
                    {
                        DateTime dateValue = (DateTime) Convert.ChangeType ( localValue, typeof ( DateTime ) );
                        string dateValueStr = dateValue.Ticks.ToString();
                        PlayerPrefs.SetString ( savedKey, dateValueStr );
                    }
                    break;
            }
        }
        void loadFromPref()
        {
            if (PlayerPrefs.HasKey(savedKey))
            {
                switch (type)
                {
                    default:
                        Debug.LogError("Pref loading not defined for this type");
                        break;
                    case SettingTypes._bool:
                        {
                            int prefValue = PlayerPrefs.GetInt(savedKey);
                            bool prefBool = (prefValue != 0);
                            localValue = (T)Convert.ChangeType(prefBool, typeof(T));
                        }
                        break;
                    case SettingTypes._int:
                        {
                            int prefValue = PlayerPrefs.GetInt(savedKey);
                            localValue = (T)Convert.ChangeType(prefValue, typeof(T));
                        }
                        break;
                    case SettingTypes._float:
                        {
                            float prefValue = PlayerPrefs.GetFloat(savedKey);
                            localValue = (T)Convert.ChangeType(prefValue, typeof(T));
                        }
                        break;
                    case SettingTypes._string:
                        {
                            string prefValue = PlayerPrefs.GetString(savedKey);
                            localValue = (T)Convert.ChangeType(prefValue, typeof(T));
                        }
                        break;
                    case SettingTypes._dateTime:
                        {
                            string prefValue = PlayerPrefs.GetString(savedKey);
                            long ticks = long.Parse(prefValue);
                            DateTime dateValue = new DateTime(ticks);
                            localValue = (T)Convert.ChangeType(dateValue, typeof(T));
                        }
                        break;
                }
            }
        }

        public enum SettingTypes
        {
            _bool,
            _int,
            _float,
            _string,
            _dateTime,
            _UNDEFINEDTYPE
        }
    }

}