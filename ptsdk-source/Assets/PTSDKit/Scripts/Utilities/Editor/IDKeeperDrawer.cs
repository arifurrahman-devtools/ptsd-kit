using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomPropertyDrawer(typeof(IDKeeper))]
    public class IDKeeperDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            //label.text = "";
            string androidID = property.FindPropertyRelative("androidID").stringValue;
            string iosID = property.FindPropertyRelative("iosID").stringValue;




            //string id = property.FindPropertyRelative("id").stringValue;
            ////int analyticsID = property.FindPropertyRelative("analyticsID").intValue;
            ////int displayID = property.FindPropertyRelative("displayID").intValue;
            //bool isBon = property.FindPropertyRelative("isBonus").boolValue;
            //string title;

            float initialOffset = 0;
            //if (isBon) initialOffset = 25;

            //int serial = LevelData.currentFocus.FindSerial(id);
            //if (serial >= 0)
            //{
            //    if (!isBon)
            //        title = string.Format("Level {0}", serial);
            //    else
            //        title = string.Format("Bonus {0}", serial);
            //}
            //else
            //    title = string.Format("Serial NOT found");


            EditorGUIUtility.labelWidth = initialOffset;

            EditorGUI.BeginProperty(position, label, property);
            if (!property.isExpanded)
            {
                bool notSet = false;
#if UNITY_ANDROID
                notSet = string.IsNullOrEmpty(androidID);
#elif UNITY_IOS
                
                notSet = string.IsNullOrEmpty(iosID);
#endif

                if (!notSet)
                {
                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
                }
                else
                {
                    GUIStyle s = new GUIStyle(EditorStyles.label);
                    s.normal.textColor = new Color(.7f,.4f,1);

                    Rect r1 = new Rect(position.x, position.y, position.width-65-20, position.height);
                    property.isExpanded = EditorGUI.Foldout(r1, property.isExpanded, label);
                    Rect r2 = new Rect(position.x + position.width - 65, position.y, 65, position.height);
                    EditorGUI.LabelField(r2, "[not set]",s);
                }

            }
            else
            {

                //var indent = EditorGUI.indentLevel;
                //EditorGUI.indentLevel = 0;

                //float btnW = 20;
                float spc = 2;
                float elementHeight = (position.height - 4) / 2;
                float elementY = position.y + elementHeight + 2;

                float offset = initialOffset;
                float fieldWidth = (position.width - 75-50-spc) / 2;



                var titleR = new Rect(position.x + offset, position.y, position.width, elementHeight);

                var label0 = new Rect(position.x + offset, elementY, 75, elementHeight);
                offset += (75);
                var item0 = new Rect(position.x + offset, elementY, fieldWidth, elementHeight);
                offset += (fieldWidth + spc);
                var label1 = new Rect(position.x + offset, elementY, 50, elementHeight);
                offset += (50 );
                var item1 = new Rect(position.x + offset, elementY, fieldWidth, elementHeight);
                //offset += (40 + spc);

                //var sceneRect = new Rect(position.x + offset, position.y, position.width - offset - btnW * 2 - spc * 2, elementHeight);

                //var removeRect = new Rect(position.x + position.width - btnW * 2 - spc, position.y, btnW, elementHeight);

                //var addRect = new Rect(position.x + position.width - btnW, position.y, btnW, elementHeight);

                //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                //EditorGUI.LabelField(titleR, label);

                property.isExpanded = EditorGUI.Foldout(titleR, property.isExpanded, label);
                EditorGUI.LabelField(label0, "androidID:");
                EditorGUI.PropertyField(item0, property.FindPropertyRelative("androidID"), GUIContent.none);
                EditorGUI.LabelField(label1, "iosID:");
                EditorGUI.PropertyField(item1, property.FindPropertyRelative("iosID"), GUIContent.none);
                //EditorGUI.PropertyField(label1, property.FindPropertyRelative("isBonus"), GUIContent.none);
                //EditorGUI.PropertyField(item1, property.FindPropertyRelative("id"), GUIContent.none);
                //EditorGUI.PropertyField(sceneRect, property.FindPropertyRelative("scene"), GUIContent.none);

                //if (LevelData.currentFocus != null)
                //{
                //    if (GUI.Button(removeRect, "-"))
                //    {
                //        LevelData.currentFocus.RemoveID(id);
                //    }
                //    if (GUI.Button(addRect, "+"))
                //    {
                //        LevelData.currentFocus.AddElement(id);
                //    }
                //}
                //EditorGUI.indentLevel = indent;
            }



            


            EditorGUI.EndProperty();

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {

                return base.GetPropertyHeight(property, label) * 2 + 4;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

    }
}

