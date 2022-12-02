using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTSDKit
{
    [CustomEditor(typeof(ABMan))]
    public class ABMan_Editor : SymbolControlledModuleEditor
    {

        bool wasDirty;

        public override string OnCoreModuleGUI()
        {

#if PTSDK_AB_TEST && PTSDK_MAX
            ABMan abman = target as ABMan;

            EditorGUI.BeginChangeCheck();
            base.OnCoreModuleGUI();

            foreach (CustomDefinitionPairs definition in abman.customDefinitionSetup.definitions)
            {
                string temp = ValidateCustomDimensionEditor(definition.customDefinition);
                definition.customDefinition = temp;
            }




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
#if PTSDK_GAME_ANALYTICS
                GameAnalyticsSDK.Setup.Settings settings = Resources.Load<GameAnalyticsSDK.Setup.Settings>("GameAnalytics/Settings");
                settings.CustomDimensions01.Clear();


                foreach (CustomDefinitionPairs definition in abman.customDefinitionSetup.definitions)
                {
                    string temp = ValidateCustomDimensionEditor(definition.customDefinition);
                    definition.customDefinition = temp;
                    settings.CustomDimensions01.Add(definition.customDefinition);
                }
                SDKEditorUtilityMan.SetObjectDirty(settings);
#endif
            }

#endif
#if !PTSDK_MAX
#if PTSDK_AB_TEST
            EditorGUILayout.HelpBox("You must have MAX sdk active to use AB testing", MessageType.Error);
#else
            EditorGUILayout.HelpBox("You must have MAX sdk active to use AB testing", MessageType.Warning);
#endif
#endif

            return "PTSDK_AB_TEST";
        }
#if PTSDK_GAME_ANALYTICS

        public static bool StringMatch(string s, string pattern)
        {
            if (s == null || pattern == null)
            {
                return false;
            }

            return System.Text.RegularExpressions.Regex.IsMatch(s, pattern);
        }
        private string ValidateCustomDimensionEditor(string customDimension)
        {
            if (customDimension.Length > 32)
            {
                Debug.LogError("Validation fail - custom dimension cannot be longer than 32 chars.");
                return "too_long";
            }
            if (!StringMatch(customDimension, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,32}$"))
            {
                if (customDimension != null)
                {
                    Debug.LogError("Validation fail - custom dimension: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: '" + customDimension + "'");
                }
                return "invalid_string";
            }
            if (ConsistsOfWhiteSpace(customDimension))
            {
                return "remove_white_space";
            }
            return customDimension;
        }

        private bool ConsistsOfWhiteSpace(string s)
        {
            foreach (char c in s)
            {
                if (c != ' ')
                    return false;
            }
            return true;
        }
#endif

        public void RefreshEnum()
        {
            ABMan abMan = target as ABMan;

            int N = abMan.AB_entries.Count;
            string[] enumEntries = new string[N];
            int k = 0;
            foreach (var abEntry in abMan.AB_entries)
            {
                abEntry.title = abEntry.title.CleanWhiteSpace();
                abEntry.ab_type = (ABtype)k;
                enumEntries[k] = string.Format("AB{0}_{1}", k, abEntry.title);
                k++;
            }


            string enumName = "ABtype";
            string filePathAndName = "Assets/PTSDKit/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("namespace PTSDKit");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tpublic enum " + enumName);
                streamWriter.WriteLine("\t{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine(string.Format("\t\t{0} = {1},", enumEntries[i], i));
                }
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
            SDKEditorUtilityMan.SetObjectDirty(abMan);
            AssetDatabase.Refresh();
            wasDirty = false;
        }
    }
}