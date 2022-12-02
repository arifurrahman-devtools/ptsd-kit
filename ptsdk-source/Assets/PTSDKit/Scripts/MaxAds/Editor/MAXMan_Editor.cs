using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(MAXMan))]
    public class MAXMan_Editor : SymbolControlledModuleEditor
    {

        public override string OnCoreModuleGUI()
        {
#if PTSDK_MAX
            base.OnCoreModuleGUI();
#endif
            return "PTSDK_MAX";
        }
    }
}