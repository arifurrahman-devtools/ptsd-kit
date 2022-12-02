using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTSDKit
{
    public class MaxBanner
    {
#if !PTSDK_MAX
        public void SetActive(bool enable)
        {

            Debug.LogError("MAXMan is not actiavetd - PTSDKit");
        }
#else
        bool log;
        Color bgColor;
        bool enabledSetByDeveloper = false;
        bool sdkThinksEnabled = false;
        public MaxBanner(string adUnitId, Color backgroundColor, bool log = false)
        {
            this.log = log;
            this.bgColor = backgroundColor;
            //Instance = this;
            this.adUnitId = adUnitId;
            InitializeBannerAds();
            Centralizer.Instance.StartCoroutine(ControlBannerRoutine());
        }
        public void InitializeBannerAds()
        {
            MaxSdk.CreateBanner(adUnitId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerBackgroundColor(adUnitId, bgColor);
        }
        IEnumerator ControlBannerRoutine()
        {
            WaitForSeconds wfs1 = new WaitForSeconds(1);
            while (true)
            {
                if (sdkThinksEnabled != bannerShouldBeEnabled)
                {
                    sdkThinksEnabled = bannerShouldBeEnabled;
                    if (sdkThinksEnabled)
                    {
                        if (log) Debug.Log("<color='magenta'>Banner enabled</color>");
                        MaxSdk.ShowBanner(adUnitId);
                        yield return wfs1;//since desyncing is dangerous
                    }
                    else
                    {
                        if (log) Debug.Log("<color='magenta'>Banner disabled</color>");
                        MaxSdk.HideBanner(adUnitId);
                        yield return wfs1;//since desyncing is dangerous
                    }
                }
                else
                {
                    if (!sdkThinksEnabled)
                    {
                        if (log) Debug.Log("<color='magenta'>Banner disable enforced</color>");
                        MaxSdk.HideBanner(adUnitId);
                        yield return wfs1;
                    }
                    else yield return null;
                }
            }
        }

        bool bannerShouldBeEnabled 
        {
            get 
            { 
                bool should = enabledSetByDeveloper
#if PTSDK_NOAD
                    && !NoAdsMan.isBlockingAds
#endif
#if PTSDK_GDPR
                    && !GDPRMan.isGDPR_flowActive
#endif
                    ;
                return should;
            }
        }


        public void SetActive(bool enable)
        {
#if PTSDK_NOAD
            if (NoAdsMan.isBlockingAds && enable)
            {
                return;
            }
#endif
            if (enabledSetByDeveloper != enable)
            {
                enabledSetByDeveloper = enable;

            }
        }
        string adUnitId = "YOUR_AD_UNIT_ID";


        
#endif
    }

}
