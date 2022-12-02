using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PTSDKit
{
    public class MaxInterstitial
    {
#if !PTSDK_MAX
        public void ShowAd()
        {
            Debug.LogError("MAXMan is not actiavetd - PTSDKit");
        }
        public bool IsReady()
        {
            Debug.LogError("MAXMan is not actiavetd - PTSDKit");
            return false;
        }
#else
        public Action onShowComplete;
        public MaxInterstitial(string adUnitId, Action onShowComplete=null)
        {
            //Instance = this;
            this.adUnitId = adUnitId;
            this.onShowComplete = onShowComplete;
            InitializeInterstitialAds();

        }
        string adUnitId = "YOUR_AD_UNIT_ID";


        int retryAttempt;
        public void ShowAd()
        {
#if PTSDK_NOAD
            if (NoAdsMan.isBlockingAds)
            {
                return;
            }
#endif
            float timeLeftTillCD = MAXMan.SkipInterstitialUntil - Time.realtimeSinceStartup;
            if (timeLeftTillCD>0)
            {
                string.Format("Interstitial Ad requests will be skipped for the next {0} seconds", timeLeftTillCD).Log(MAXMan.Instance.LogColorCode);
                return;
            }

            if (MaxSdk.IsInterstitialReady(adUnitId)) MaxSdk.ShowInterstitial(adUnitId);
        }

        public bool IsReady()
        {
            return MaxSdk.IsInterstitialReady(adUnitId);
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(adUnitId);
        }
        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

            // Load the first interstitial
            LoadInterstitial();
        }



        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            retryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            Centralizer.Add_DelayedAct(LoadInterstitial, (float)retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
            onShowComplete?.Invoke();
        }
#endif
    }
}
