using System;

namespace Asteroids
{
    public interface IAdService
    {
        void Initialize();
        void ShowRewardedAd(Action onRewardGranted);
        void ShowInterstitialAd(Action onInterstitialComplete);
        bool IsRewardedAdReady();

    }
}
