using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTSDKit
{
    public static class StringDebug
    {
        public static void Log(this string str)
        {
            Debug.Log(str);
        }
        public static void Log(this string str, string hexColorCode)
        {
            Debug.LogFormat("<color=#{0}>{1}</color>",hexColorCode,str);
        }
    }

}