using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chassis.Core
{
    /// <summary>
    /// Mock/Console-based implementation of Ads Provider for testing.
    /// Simulates successful ad loading and 100% success rewarded claims.
    /// </summary>
    public class DummyAdsProvider : IAdsProvider
    {
        public void Initialize(Action onComplete)
        {
            Debug.Log("[Ads] Dummy Ad Mediation successfully initialized.");
            onComplete?.Invoke();
        }

        public void ShowInterstitial(string placement, Action onClosed)
        {
            Debug.Log($"[Ads] Showing Interstitial mock-up for placement: <b>{placement}</b>");
            
            // Log ad offer & show analytics
            LogAdAnalytics(placement, "interstitial", true);

            // Simulating instant close
            onClosed?.Invoke();
        }

        public void ShowRewarded(string placement, Action<bool> onRewardEarned)
        {
            Debug.Log($"[Ads] Showing Rewarded mock-up for placement: <b>{placement}</b>");

            // Log ad offer & show analytics
            LogAdAnalytics(placement, "rewarded", true);

            // 100% successful claim simulation
            Debug.Log($"[Ads] Rewarded succeeded. Triggering reward callback for placement: <b>{placement}</b>");
            
            // Log reward analytics
            LogAdRewardAnalytics(placement);

            onRewardEarned?.Invoke(true);
        }

        public bool IsInterstitialReady(string placement) => true;

        public bool IsRewardedReady(string placement) => true;

        private void LogAdAnalytics(string placement, string adType, bool wasShown)
        {
            if (ServiceLocator.TryGet<IAnalyticsProvider>(out var analytics))
            {
                var offerParams = new Dictionary<string, object>
                {
                    { AnalyticsEvents.Params.Placement, placement },
                    { AnalyticsEvents.Params.AdType, adType }
                };
                analytics.LogEvent(AnalyticsEvents.AdOffer, offerParams);

                if (wasShown)
                {
                    analytics.LogEvent(AnalyticsEvents.AdShown, offerParams);
                }
            }
        }

        private void LogAdRewardAnalytics(string placement)
        {
            if (ServiceLocator.TryGet<IAnalyticsProvider>(out var analytics))
            {
                var rewardParams = new Dictionary<string, object>
                {
                    { AnalyticsEvents.Params.Placement, placement }
                };
                analytics.LogEvent(AnalyticsEvents.AdReward, rewardParams);
            }
        }
    }
}
