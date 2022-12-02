using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(FireBaseMan))]
    public class FireBaseMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if PTSDK_FIREBASE
            base.OnCoreModuleGUI();
#endif 
            return "PTSDK_FIREBASE";
        }
    }
}