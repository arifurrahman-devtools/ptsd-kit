using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(AdjustMan))]
    public class AdjustMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if PTSDK_ADJUST
            base.OnCoreModuleGUI();

            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            GUILayout.BeginHorizontal();
            GUILayout.Label(AdjustMan.isTestMode ? "*Test Mode Active*" : "Production Mode Active");
            if (AdjustMan.isTestMode)
            {
                if (GUILayout.Button("Test=>Production"))
                {
                    string oldbundleid = EditorPrefs.GetString("STORED_BUNDLE_ID", "");
                    string body = string.Format("PTSDKit will attempt to restore your bundleId to {0} However, you should manually refresh your Player setting window to check", oldbundleid);
                    if (EditorUtility.DisplayDialog("Returning to production mode", body, "Proceed", "Cancel"))
                    {
                        Debug.LogFormat("TestMode Terminated. Restoring bundle id to {0}", oldbundleid);
                        Debug.Log("You should consider delete and resolving your android libraries after setting Package Name");
                        PlayerSettings.SetApplicationIdentifier(buildTargetGroup, oldbundleid);
                        //SetProjDirty();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorApplication.ExecuteMenuItem("File/Save Project");
                    }

                }
            }
            else
            {
                if (GUILayout.Button("Production=>Test", GUILayout.Height(25), GUILayout.MinWidth(75)))
                {
                    if (EditorUtility.DisplayDialog("Enabling Test Mode", $"PTSDKit will change your bundle ID to {AdjustMan.testbundleid} for testing", "Proceed", "Cancel"))
                    {
                        EditorPrefs.SetString("STORED_BUNDLE_ID", PlayerSettings.applicationIdentifier);
                        PlayerSettings.SetApplicationIdentifier(buildTargetGroup, AdjustMan.testbundleid);
                        //SetProjDirty();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorApplication.ExecuteMenuItem("File/Save Project");
                    }

                }
            }
            GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();

#endif

            return "PTSDK_ADJUST";
        }

    }
}

