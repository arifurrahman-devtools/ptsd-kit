using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(GameAnalyticsMan))]
    public class GameAnalyticsMan_Editor : SymbolControlledModuleEditor
    {

        public override string OnCoreModuleGUI()
        {
#if PTSDK_GAME_ANALYTICS
            GameAnalyticsMan ga = (GameAnalyticsMan)target;
            if (!ga.prefabReference)
            {
                ga.prefabReference = AssetDatabase.LoadMainAssetAtPath("Assets/GameAnalytics/Plugins/Prefabs/GameAnalytics.prefab") as GameObject;
                SDKEditorUtilityMan.SetObjectDirty(ga);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorApplication.ExecuteMenuItem("File/Save Project");
                if (!ga.prefabReference) EditorGUILayout.HelpBox("Please assign GA prefab if not auto assigned", MessageType.Error);
            }
            base.OnCoreModuleGUI();
#endif
            return "PTSDK_GAME_ANALYTICS";
        }
    }
}