
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(AppsFlyerMan))]
    public class AppsFlyerMan_Editor : SymbolControlledModuleEditor
    {

        public override string OnCoreModuleGUI()
        {
#if PTSDK_APPSFLYER
            base.OnCoreModuleGUI();
#endif
            return "PTSDK_APPSFLYER";
        }
    }
}