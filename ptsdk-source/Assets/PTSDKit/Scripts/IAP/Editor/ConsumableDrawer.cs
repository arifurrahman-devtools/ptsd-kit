using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
#if PTSDK_IAP
    [CustomPropertyDrawer(typeof(ConsumablePTSDItem))]
    public class ConsumableDrawer : PurchasableDrawer
    {
        protected override void OnPreIDFields(Rect position,SerializedProperty property)
        {
            var LRect = new Rect(position.x, position.y + offset.y, position.width/2, lineHeight);
            offset += new Vector2(0, 0);
            var VRect = new Rect(position.x+ position.width / 2, position.y + offset.y, position.width/2, lineHeight);
            offset += new Vector2(0, elementHeight);
            EditorGUI.LabelField(LRect, "Reward Per Perchase");
            EditorGUI.PropertyField(VRect, property.FindPropertyRelative("RewardPerPurchase"), GUIContent.none);
        }
        protected override float GetAdditionalHeight()
        {
            return elementHeight;
        }
    }
#endif
}

