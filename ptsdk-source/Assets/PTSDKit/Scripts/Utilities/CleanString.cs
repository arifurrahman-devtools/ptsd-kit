using System;
using UnityEngine;
public static class CleanString
{

    public static string CleanWhiteSpace(this string s)
    {
        string[] ar = s.Split(' ');
        string fin = "";
        for (int i = 0; i < ar.Length; i++)
        {
            fin = string.Concat(fin, ar[i]);
        }
        Debug.Log(fin);
        return fin;
    }
    public static bool Compare(this string og, string header_ontarget, string target)
    {
        target = string.Concat(header_ontarget, target);
        return string.Equals(og, target);
    }
}