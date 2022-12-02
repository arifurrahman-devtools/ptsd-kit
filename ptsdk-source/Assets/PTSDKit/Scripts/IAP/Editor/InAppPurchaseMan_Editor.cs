using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTSDKit
{
    [CustomEditor(typeof(InAppPurchaseMan))]
    public class InAppPurchaseMan_Editor : SymbolControlledModuleEditor
    {
        bool wasDirty;

        public override string OnCoreModuleGUI()
        {

#if PTSDK_IAP
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ActiveLogLevel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableTestUI"));
            EditorGUI.BeginChangeCheck();
            //base.OnInspectorGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nonConsumablePurchases"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("consumablePurchases"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("helperCanvasPrefab"));
            if (EditorGUI.EndChangeCheck())
            {
                wasDirty = true;

            }
            serializedObject.ApplyModifiedProperties();
            if (wasDirty)
            {
                if (GUILayout.Button("Force Refresh Enum", GUILayout.Height(35)))
                {
                    RefreshEnum();
                }
            }
#endif

            return "PTSDK_IAP";
        }

#if PTSDK_IAP
        private void OnDisable()
        {
            if (wasDirty)
            {
                RefreshEnum();
                EditorUtility.DisplayDialog("Must Recompile!", "Just made change to the in app purchase Enum. Please give unity some time :(", "OK");
            }
        }


        public void RefreshEnum()
        {
            InAppPurchaseMan iapMan = target as InAppPurchaseMan;

            int N = iapMan.consumablePurchases.Count + iapMan.nonConsumablePurchases.Count;
            string[] enumEntries = new string[N];
            int k = 0;
            foreach (var item in iapMan.nonConsumablePurchases)
            {
                item.title = item.title.CleanWhiteSpace();
                item.typeID = k;
                enumEntries[k] = string.Format("iap{0}_{1}", k, item.title);
                k++;
            }
            foreach (var item in iapMan.consumablePurchases)
            {
                item.title = item.title.CleanWhiteSpace();
                enumEntries[k] = string.Format("iap{0}_{1}",k, item.title);
                item.typeID = k;
                k++;
            }

            string enumName = "PurchaseType";
            string filePathAndName = "Assets/PTSDKit/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("namespace PTSDKit");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tpublic enum " + enumName);
                streamWriter.WriteLine("\t{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine(string.Format( "\t\t{0} = {1},", enumEntries[i],i));
                }
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
            SDKEditorUtilityMan.SetObjectDirty(iapMan);
            AssetDatabase.Refresh();
            wasDirty = false;
        }
#endif

    }


}