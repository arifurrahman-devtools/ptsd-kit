using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(FaceBookMan))]
    public class FaceBookMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if PTSDK_FACEBOOK
            base.OnCoreModuleGUI();
#endif
            return "PTSDK_FACEBOOK";
        }
    }
}