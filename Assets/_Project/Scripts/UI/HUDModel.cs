using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class HUDModel : IDisposable
    {
        public event Action<Vector2, float> OnCoordinatesUpdated;
        public event Action<float> OnSpeedUpdated;
        public event Action<int> OnCurrentShotsUpdated;
        public event Action<float> OnRechargeTimerUpdated;
        public event Action OnPlayerDead;
        public event Action<int> OnScoreChanged;
        public event Action OnRevive;

        private int _maxShots;
        private int _currentShots;
        private SceneService _sceneService;
        private ScoreCounter _scoreCounter;
        private PauseGame _pauseManager;
        private SaveData _saveData;
        private IAdService _adService;

        [Inject]
        public HUDModel(SceneService ss, ScoreCounter sc, PauseGame pm, SaveData sd, IAdService adService)
        {
            _sceneService = ss;
            _scoreCounter = sc;
            _saveData = sd;
            _pauseManager = pm;
            _adService = adService;

            _scoreCounter.OnScoreChanged += UpdateScore;
        }

        public void UpdateScore(int score)
        {
            OnScoreChanged?.Invoke(score);
        }

        public void SetMaxLaserShots(int maxShots)
        {
            _maxShots = maxShots;
        }

        public void UpdateCoordinates(Vector2 coords, float angle)
        {
            OnCoordinatesUpdated?.Invoke(coords, angle);
        }

        public void UpdateSpeed(float speed)
        {
            OnSpeedUpdated?.Invoke(speed);
        }

        public void UpdateCurrentShots(int current)
        {
            _currentShots = current;
            OnCurrentShotsUpdated?.Invoke(_currentShots);
        }

        public void UpdateRechargeTimer(float time)
        {
            OnRechargeTimerUpdated?.Invoke(time);
        }

        public void PlayerDead()
        {
            OnPlayerDead?.Invoke();
            _saveData.SaveScore();
            _pauseManager.PauseGameProcess();
        }

        public void RequestReloadGame()
        {
            _adService.ShowInterstitialAd(ReloadSessionAfterAd);
        }

        public void RequestExitGame()
        {
            _sceneService.ExitGame();
        }

        public void RequestRewardedAd()
        {
            if(_saveData.HasUsedRevive == true)
            {
                return;
            }

            _saveData.UseRevive();
            _adService.ShowRewardedAd(ContinueSessionAfterAd);
        }

        private void ReloadSessionAfterAd()
        {
            _sceneService.ReloadGame();
        }

        private void ContinueSessionAfterAd()
        {
            _pauseManager.PauseGameProcess();
            OnRevive?.Invoke();
            _saveData.UseRevive();
            Debug.Log("Player revived after rewarded ad!");
        }

        public void Dispose()
        {
            _scoreCounter.OnScoreChanged -= UpdateScore;
        }

        public int MaxShots => _maxShots;
        public int CurrentShots => _currentShots;
    }
}
