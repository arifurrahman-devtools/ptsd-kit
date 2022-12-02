using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTSDKit
{
    [CustomEditor(typeof(AdjustIAPVerifyMan))]
    public class AdjPurchaseMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {

#if PTSDK_IAP_VALIDATION
            base.OnCoreModuleGUI();

#endif
            return "PTSDK_IAP_VALIDATION";
        }
    }
}