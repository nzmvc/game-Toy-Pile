using System;

namespace Chassis.Core
{
    /// <summary>
    /// Contract for wrapping third-party ad networks and mediation suites (such as AppLovin Max or Unity LevelPlay).
    /// </summary>
    public interface IAdsProvider
    {
        void Initialize(Action onComplete);
        void ShowInterstitial(string placement, Action onClosed);
        void ShowRewarded(string placement, Action<bool> onRewardEarned);
        bool IsInterstitialReady(string placement);
        bool IsRewardedReady(string placement);
    }
}
