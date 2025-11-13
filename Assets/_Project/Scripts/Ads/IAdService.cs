using System;

namespace Asteroids
{
    public interface IAdService
    {
        void ShowRewardedAd(Action onRewardGranted);
        void ShowInterstitialAd(Action onInterstitialComplete);
        bool IsRewardedAdReady();

    }
}
