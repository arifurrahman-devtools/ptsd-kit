using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(GDPRMan))]
    public class GDPRMan_Editor : SymbolControlledModuleEditor
    {
        public string temp_name;
        GDPRMan t;
        GameObject flow_pref;
        GDPRFlowController flow_controller;
        private void OnEnable()
        {
            GDPRMan t = target as GDPRMan;
            flow_pref = t.gdprPrefab;
            flow_controller = flow_pref.GetComponent<GDPRFlowController>();
        }
        public override string OnCoreModuleGUI()
        {
#if PTSDK_GDPR
            base.OnCoreModuleGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Replace Game Name in GDPR");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(new GUIContent("Current Game Name"), flow_controller.currentGameName);
            EditorGUI.EndDisabledGroup();

            temp_name = EditorGUILayout.TextField(new GUIContent("New Game Name"), temp_name);
            if (!string.IsNullOrEmpty(temp_name))
            {
                if (GUILayout.Button("Set Game Name for GDPR flow"))
                {
                    string path = "Assets/PTSDKit/SystemPrefabs/GDPR_canvas.prefab";
                    GameObject go = PrefabUtility.LoadPrefabContents(path);
                    go.GetComponent<GDPRFlowController>().SetName(temp_name);
                    PrefabUtility.SaveAsPrefabAsset(go, path);
                    PrefabUtility.UnloadPrefabContents(go);
                    temp_name = "";
                }
            }


#endif
            return "PTSDK_GDPR";
        }
    }
}