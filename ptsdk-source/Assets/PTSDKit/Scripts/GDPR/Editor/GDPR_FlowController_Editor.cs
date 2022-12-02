using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PTSDKit
{
    [CustomEditor(typeof(GDPRFlowController))]
    public class GDPR_FlowController_Editor : Editor
    {
        public string temp_name;
        public override void OnInspectorGUI()
        {
            GDPRFlowController t = (target as GDPRFlowController);
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Replace Game Name in GDPR");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGameName"));
            EditorGUI.EndDisabledGroup();

            temp_name = EditorGUILayout.TextField(new GUIContent("New Game Name"), temp_name);
            if (!string.IsNullOrEmpty(temp_name))
            {
                if (GUILayout.Button("Set Game Name for GDPR flow"))
                {
                    t.SetName(temp_name);
                    SDKEditorUtilityMan.SetObjectDirty(t);
                    temp_name = "";
                }
            }

        }
    }
}