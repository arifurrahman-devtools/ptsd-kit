using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(QuickLinkButton))]
    public class QuickLinkButton_Editor : Editor
    {
        public string temp_link;
        public override void OnInspectorGUI()
        {
            QuickLinkButton t = (target as QuickLinkButton);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("link"));
            EditorGUI.EndDisabledGroup();
            temp_link = EditorGUILayout.TextField(new GUIContent("New Link"), temp_link);
            if (!string.IsNullOrEmpty(temp_link))
            {
                if (GUILayout.Button("Set New Link"))
                {
                    t.SetLink(temp_link);
                    SDKEditorUtilityMan.SetObjectDirty(t);
                    temp_link = "";
                }
            }

        }
    }

}
