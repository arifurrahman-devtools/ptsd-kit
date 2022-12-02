using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomPropertyDrawer(typeof(ABDisplayDefinitions))]
    public class ABDisplayDefinitions_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position,label,property);
            float initialOffset = 0;


            EditorGUIUtility.labelWidth = initialOffset;



            float spc = 4;
            float elementHeight = (position.height - 4);// / 2;
            float elementY = position.y;// + elementHeight + 2;

            float offset = initialOffset;
            float fieldWidth = (position.width - 75 - 45 - spc*3) / 2;


            var label0 = new Rect(position.x + offset, elementY, 45, elementHeight);
            offset += (45 + spc);
            var item0 = new Rect(position.x + offset, elementY, fieldWidth, elementHeight);
            offset += (fieldWidth + spc);
            var label1 = new Rect(position.x + offset, elementY, 75, elementHeight);
            offset += (75 + spc);
            var item1 = new Rect(position.x + offset, elementY, fieldWidth, elementHeight);


            EditorGUI.LabelField(label0, "value:");
            EditorGUI.PropertyField(item0, property.FindPropertyRelative("value"), GUIContent.none);
            EditorGUI.LabelField(label1, "significance:");
            EditorGUI.PropertyField(item1, property.FindPropertyRelative("significance"), GUIContent.none);


            




            EditorGUI.EndProperty();

        }


    }
}

