using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTSDKit
{
    public class GDPRMan : MonoBehaviour, IEarlyInitiatable
    {
        public static bool isGDPR_flowActive { get; private set; }
        public static bool ConsentApplies { get; private set; }
        public static bool IsPermissionPending { get { return !(analyticsState.value && adsState.value); } }
        public static bool HasAdPermission { get { return adsState.value; } }
        public static bool HasAnalyticsPermission { get { return analyticsState.value; } }
        public static GDPRMan Instance { get; private set; }
        public static Action onGDPR_FlowEnableDisable;

        static HardData<bool> analyticsState;
        static HardData<bool> adsState;
        static HardData<bool> choiceConfirmed;
        static HardData<bool> adjustForgot;

        static GDPRFlowController gdprCanvas;
        static Action<IEarlyInitiatable> ReportModuleIsReady;

        public GameObject gdprPrefab;
        public bool waitForGDPRFlow = false;
        [Space]
        [Space]
        public bool enforceConsentFlow = false;
        public List<string> partnerLinks;


        bool enableModuleLogs = true;
        public string LogColorCode => "440088";

        public bool IsReady { get; set; }

        void IEarlyInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }

        void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, Action<IEarlyInitiatable> onModuleReadyToUse)
        {
#if PTSDK_GDPR
            ConsentApplies = (ControlCenter.consentDialogApplicableDetected && !ControlCenter.ios14_5plusDetected) || enforceConsentFlow;
            Debug.Log("caaaaaaaaaaaa"+ConsentApplies);
            if (!waitForGDPRFlow)
            {
                onModuleReadyToUse?.Invoke(this);
                if (!ConsentApplies) return;
            }
            else
            {
                if (!ConsentApplies)
                {
                    onModuleReadyToUse?.Invoke(this);
                    return;
                }
                GDPRMan.ReportModuleIsReady = onModuleReadyToUse;
            }


            Instance = this;
            analyticsState = new HardData<bool>("GDPR_ANALYTICS", true);
            adsState = new HardData<bool>("GDPR_ADS", true);
            choiceConfirmed = new HardData<bool>("GDPR_CHOSEN", false);
            adjustForgot = new HardData<bool>("GDPR_ADJUST_FORGOT", false);

            gdprCanvas = Instantiate(gdprPrefab).GetComponent<GDPRFlowController>();
            gdprCanvas.transform.SetParent(this.transform);
            gdprCanvas.PopulateLinks(partnerLinks);
            if (!choiceConfirmed.value)
            {
                gdprCanvas.StartFlow(OnGDPRFlowResult);
                isGDPR_flowActive = true;
                onGDPR_FlowEnableDisable?.Invoke();
            }
            else
            {
                if (IsPermissionPending)
                {
                    gdprCanvas.StartFlow(OnGDPRFlowResult);
                    isGDPR_flowActive = true;
                    onGDPR_FlowEnableDisable?.Invoke();
                }
                else
                {
                    gdprCanvas.Disable();
                    ReportModuleIsReady?.Invoke(this);
                    ReportModuleIsReady = null;
                    IsReady = true;
                }
            }
#else
            onModuleReadyToUse?.Invoke(this);
#endif
        }

        static void OnGDPRFlowResult(bool analytics, bool ads) 
        {
            choiceConfirmed.value = true;

            analyticsState.value = analytics;
            adsState.value = ads;

#if PTSDK_MAX
            MaxSdk.SetHasUserConsent(ads);
#endif

#if PTSDK_ADJUST
            if (!analytics && !adjustForgot.value)
            {
                com.adjust.sdk.Adjust.gdprForgetMe();
                adjustForgot.value = true;
            }
#endif

            ReportModuleIsReady?.Invoke(Instance);
            ReportModuleIsReady = null;
            Instance.IsReady = true;


            gdprCanvas.disruptiveButton.gameObject.SetActive(IsPermissionPending);
            gdprCanvas.disruptiveButton.onClick.RemoveAllListeners();
            gdprCanvas.disruptiveButton.onClick.AddListener(() => {

                gdprCanvas.ReturnToFlow(OnGDPRFlowResult);
                isGDPR_flowActive = true;
                onGDPR_FlowEnableDisable?.Invoke();
            });
            isGDPR_flowActive = false;
            onGDPR_FlowEnableDisable?.Invoke();
        }

        public static void RequestGDPRFlow()
        {
            gdprCanvas.ReturnToFlow(OnGDPRFlowResult);
            isGDPR_flowActive = true;
            onGDPR_FlowEnableDisable?.Invoke();
        }

    }
}

