using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTSDKit
{
    [System.Serializable]
    public class IDKeeper
    {
        public static implicit operator string (IDKeeper keep) => keep.id;

        [SerializeField] string androidID;
        [SerializeField] string iosID;

        public IDKeeper() { }
        public IDKeeper(string androidID, string iosID) 
        {
            this.androidID = androidID;
            this.iosID = iosID;
        }
        public bool isValid
        {
            get
            {
                return !string.IsNullOrEmpty(id);
            }
        }
        public string id
        {
            get
            {
#if UNITY_IOS
                return iosID;
#elif UNITY_ANDROID
                return androidID;
#else
                Debug.LogError("Invalid Platform");
                return null;
#endif

            }
        }
    }
}