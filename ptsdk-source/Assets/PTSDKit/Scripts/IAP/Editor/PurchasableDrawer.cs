using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{

#if PTSDK_IAP
    [CustomPropertyDrawer(typeof(PurchaseablePTSDItem))]
    public class PurchasableDrawer : PropertyDrawer
    {
        bool justSelected = true;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool autoGen = !property.FindPropertyRelative("disableAutoGeneration").boolValue;
            string title = property.FindPropertyRelative("title").stringValue;

            //string androidID = property.FindPropertyRelative("androidID").stringValue;
            //string iosID = property.FindPropertyRelative("iosID").stringValue;




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

            //EditorGUI.BeginFoldoutHeaderGroup(position, true, label.text);
            if (!string.IsNullOrEmpty(title)) label.text = title;
            if (!property.isExpanded)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            }
            else
            {

                var indent = EditorGUI.indentLevel;
                offset.x = initialOffset;
                offset.y = 0;

                var expandR = new Rect(position.x + offset.x, position.y + offset.y, position.width, lineHeight);
                offset += new Vector2(0, elementHeight+vPad);

                property.isExpanded = EditorGUI.Foldout(expandR, property.isExpanded, label);

                var titleLRect = new Rect(position.x + offset.x, position.y + offset.y, 35, lineHeight);
                offset += new Vector2(35 + hPad, 0);
                float w = position.width - offset.x - 120 - 20 - hPad * 2;
                var titleVRect = new Rect(position.x + offset.x, position.y + offset.y, w, lineHeight);
                offset += new Vector2(w + hPad, 0);

                var autoLRect = new Rect(position.x + offset.x, position.y + offset.y, 120, lineHeight);
                offset += new Vector2(120 + hPad, 0);
                var autoVRect = new Rect(position.x + offset.x, position.y + offset.y, 20, lineHeight);
                offset += new Vector2(-offset.x, elementHeight);

                EditorGUI.LabelField(autoLRect, "Disable Auto ID Gen");
                EditorGUI.PropertyField(autoVRect, property.FindPropertyRelative("disableAutoGeneration"), GUIContent.none);
                EditorGUI.LabelField(titleLRect, "Title");
                EditorGUI.PropertyField(titleVRect, property.FindPropertyRelative("title"), GUIContent.none);


                OnPreIDFields(position, property);

                SerializedProperty sp_productID = property.FindPropertyRelative("productID");
                if (autoGen)
                {
                    sp_productID.FindPropertyRelative("androidID").stringValue = title;
                    sp_productID.FindPropertyRelative("iosID").stringValue = string.Format("{0}.{1}", Application.identifier, title);
                }
                int lines = 0;
                if (justSelected) sp_productID.isExpanded = true;
                lines = sp_productID.isExpanded ? 2 : 1;


                var productIDRect = new Rect(position.x, position.y + offset.y, position.width, elementHeight * lines);
                offset += new Vector2(0, elementHeight * lines);

                EditorGUI.BeginDisabledGroup(autoGen);
                //EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(productIDRect, sp_productID, new GUIContent("Product IDs"));
                EditorGUI.EndDisabledGroup();

#if PTSDK_IAP_VALIDATION
                SerializedProperty sp_adjustTokens = property.FindPropertyRelative("adjustEventToken");


                if (justSelected) sp_adjustTokens.isExpanded = true;
                lines = sp_adjustTokens.isExpanded ? 2 : 1;
                var adjustTokenRect = new Rect(position.x, position.y + offset.y, position.width, elementHeight * lines);
                offset += new Vector2(0, elementHeight * lines);

                //EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(adjustTokenRect, sp_adjustTokens, new GUIContent("Adjust Event Tokens"));
#endif
                EditorGUI.indentLevel = indent;
            }

            EditorGUI.EndProperty();

            if (justSelected) justSelected = false;

        }

        protected Vector2 offset;
        protected float elementHeight { get { return lineHeight + vPad; } }
        protected const float lineHeight = 18;
        //const float lineGap = 2;
        protected const float vPad = 3;
        protected const float hPad = 4;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                SerializedProperty sp_productID = property.FindPropertyRelative("productID");
                SerializedProperty sp_adjustTokens = property.FindPropertyRelative("adjustEventToken");
                int lineCount = 3;
                if (sp_productID.isExpanded) lineCount++;
#if PTSDK_IAP_VALIDATION
                lineCount += sp_adjustTokens.isExpanded ? 2 : 1;
#endif

                return elementHeight * lineCount + vPad * 2 + GetAdditionalHeight();
            }
            else
            {
                return elementHeight;
            }    

        }
        protected virtual void OnPreIDFields(Rect position, SerializedProperty property)
        {

        }
        protected virtual float GetAdditionalHeight()
        {
            return 0;
        }
    }
#endif
}

