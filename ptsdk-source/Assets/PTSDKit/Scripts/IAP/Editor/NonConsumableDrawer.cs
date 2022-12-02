using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
#if PTSDK_IAP
    [CustomPropertyDrawer(typeof(NonConsumablePTSDItem))]
    public class NonConsumableDrawer : PurchasableDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }

    }
#endif
}

