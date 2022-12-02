using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(ByteBrewMan))]
    public class ByteBrewMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if PTSDK_BYTEBREW

            ByteBrewMan ga = (ByteBrewMan)target;
            if (!ga.prefabReference)
            {
                ga.prefabReference = AssetDatabase.LoadMainAssetAtPath("Assets/ByteBrewSDK/Prefabs/ByteBrew.prefab") as GameObject;
                SDKEditorUtilityMan.SetObjectDirty(ga);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorApplication.ExecuteMenuItem("File/Save Project");
                if (!ga.prefabReference) EditorGUILayout.HelpBox("Please assign ByteBrew prefab if not auto assigned", MessageType.Error);
            }

            base.OnCoreModuleGUI();
#endif
            return "PTSDK_BYTEBREW";
        }
    }
}