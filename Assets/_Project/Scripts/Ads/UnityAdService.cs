using System;
using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;

namespace Asteroids
{
    public class UnityAdService : IAdService, IInitializable, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private Action currentRewardCallback;
        private Action currentInterstitialCallback;

        private string _androidGameId = "5977227";
        private string _iosGameId = "5977226";
        private bool _testMode = true;

        private string _androidRewardedAdUnitId = "Rewarded_Android";
        private string _iosRewardedAdUnitId = "Rewarded_iOS";
        private string _androidInterstitialAdUnitId = "Interstitial_Android";
        private string _iosInterstitialAdUnitId = "Interstitial_iOS";

        private string _gameId;
        private string _rewardedAdUnitId;
        private string _interstitialAdUnitId;
        private bool _isRewardedReady = false;
        private StoredDataHandler _saveData;

        [Inject]
        public void Construct(StoredDataHandler saveData)
        {
            _saveData = saveData;
        }

        public void Initialize()
        {

#if UNITY_IOS
            _gameId = iosGameId;
            _rewardedAdUnitId = iosRewardedAdUnitId;
            _interstitialAdUnitId = iosInterstitialAdUnitId;
#elif UNITY_ANDROID
            _gameId = _androidGameId;
            _rewardedAdUnitId = _androidRewardedAdUnitId;
            _interstitialAdUnitId = _androidInterstitialAdUnitId;
#elif UNITY_EDITOR
            _gameId = _androidGameId;
            _rewardedAdUnitId = _androidRewardedAdUnitId;
            _interstitialAdUnitId = _androidInterstitialAdUnitId;
#endif

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialized.");
            LoadAds();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads init failed. Error: {error}. Message: {message}");
        }

        public void ShowInterstitialAd(Action onInterstitialComplete)
        {
            if (_saveData.HasAdBlock)
            {
                onInterstitialComplete?.Invoke();
                return;
            }

            currentInterstitialCallback = onInterstitialComplete;
            Advertisement.Show(_interstitialAdUnitId, this);
        }

        public void ShowRewardedAd(Action onRewardGranted)
        {
            currentRewardCallback = onRewardGranted;
            Advertisement.Show(_rewardedAdUnitId, this);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Ad loaded: {placementId}");

            if (placementId == _rewardedAdUnitId)
            {
                _isRewardedReady = true;
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Failed to load ad {placementId}. Error: {error}. Message: {message}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (placementId == _rewardedAdUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Rewarded ad completed — grant reward.");
                currentRewardCallback?.Invoke();
                currentRewardCallback = null;
            }

            if (placementId == _interstitialAdUnitId)
            {
                currentInterstitialCallback?.Invoke();
                currentInterstitialCallback = null;
            }

            if (placementId == _rewardedAdUnitId)
            {
                Advertisement.Load(_rewardedAdUnitId, this);
            }
            else if (placementId == _interstitialAdUnitId)
            {
                Advertisement.Load(_interstitialAdUnitId, this);
            }
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Show failed for {placementId}. Error: {error}. Message: {message}");
            LoadAds();
        }

        public bool IsRewardedAdReady()
        {
            return _isRewardedReady;
        }

        public void OnUnityAdsShowClick(string placementId) { }

        public void OnUnityAdsShowStart(string placementId) { }

        private void LoadAds()
        {
            Advertisement.Load(_rewardedAdUnitId, this);
            Advertisement.Load(_interstitialAdUnitId, this);
        }
    }
}
