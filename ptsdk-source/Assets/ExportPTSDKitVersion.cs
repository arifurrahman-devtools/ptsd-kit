using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
public class ExportPTSDKitVersion : EditorWindow
{
    [MenuItem("PTSD/Sign PTSDKit", priority = 50)]
    static void CreateProjectCreationWindow()
    {
        ExportPTSDKitVersion window = new ExportPTSDKitVersion();
        window.ShowUtility();
        window.version = ReadVersionFile();
    }

    public string editorWindowText = "Choose a project name: ";
    string version ="x.x.x";

    void OnGUI()
    {
        string savedVersion = ReadVersionFile();
        //this.titleContent.text = $"Current Version: {savedVersion}";

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Current Version:", $"{savedVersion}");
        EditorGUI.EndDisabledGroup();
        version = EditorGUILayout.TextField("Version:", version);
        //this.Repaint();

        //if (GUILayout.Button($"Update {savedVersion}=>{version}"))
        //{
        //    WriteVersionFile(version);
        //}
        version = version.Trim();
        if (GUILayout.Button($"Export {version}"))
        {
            WriteVersionFile(version);
            Export(version);
        }
        if (GUILayout.Button("Abort"))
            Close();
    }

    static void WriteVersionFile(string version)
    {
        string path = "Assets/PTSDKit/PTSDKit_version.txt";

        string data = $"{version}";
        StreamWriter sw = File.CreateText(path);
        sw.WriteLine(data);
        sw.Close();
        //Debug.LogErrorFormat(data);
    }
    static string ReadVersionFile()
    {
        string path = "Assets/PTSDKit/PTSDKit_version.txt";
        if (File.Exists(path))
        {
            return File.ReadAllText(path).Trim();
        }
        else return "0.0.0";
    }

    static void Export(string version)
    {
        var exportedPackageAssetList = new List<string>();
        exportedPackageAssetList.Add("Assets/PTSDKit");
        Debug.LogError(version);
        //Export Shaders and Prefabs with their dependencies into a .unitypackage
        AssetDatabase.ExportPackage(exportedPackageAssetList.ToArray(), $"PTSDKit_{version}.unitypackage",
            ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
    }


}
#endif