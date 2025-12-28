using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class HUDModel : IInitializable, IDisposable
    {
        public event Action<Vector2, float> OnCoordinatesUpdated;
        public event Action<float> OnSpeedUpdated;
        public event Action<int> OnCurrentShotsUpdated;
        public event Action<float> OnRechargeTimerUpdated;
        public event Action OnPlayerDead;
        public event Action<int> OnScoreChanged;
        public event Action OnRevive;
        public event Action OnNewRecord;
        public event Action<int> OnBestScoreSetup;

        private int _maxShots;
        private int _currentShots;
        private SceneService _sceneService;
        private ScoreCounter _scoreCounter;
        private PauseGame _pauseManager;
        private StoredDataHandler _storedDataHandler;
        private IAdService _adService;
        private int _currentBestScore;

        [Inject]
        public HUDModel(SceneService sceneService, ScoreCounter scoreCounter, PauseGame pauseManager, StoredDataHandler storedDataHandler, IAdService adService)
        {
            _sceneService = sceneService;
            _scoreCounter = scoreCounter;
            _storedDataHandler = storedDataHandler;
            _pauseManager = pauseManager;
            _adService = adService;
        }        

        public void Initialize()
        {
            _scoreCounter.OnScoreChanged += UpdateScore;
            _storedDataHandler.OnChoosePlaceToSave += ReachNewRecord;
            _storedDataHandler.OnSaveScoreAfterDeath += PlayerDeath;
            
            DownloadBestScore().Forget();
        }
        
        public void Dispose()
        {
            _scoreCounter.OnScoreChanged -= UpdateScore;
            _storedDataHandler.OnChoosePlaceToSave -= ReachNewRecord;
            _storedDataHandler.OnSaveScoreAfterDeath -= PlayerDeath;
        }

        private async UniTaskVoid DownloadBestScore()
        {
            _currentBestScore = await _storedDataHandler.GetBestScoreAndUpdateData();
            OnBestScoreSetup?.Invoke(_currentBestScore);
        }

        private void UpdateScore(int score) => OnScoreChanged?.Invoke(score);

        public void SetMaxLaserShots(int maxShots) => _maxShots = maxShots;


        public async UniTask PlayerDead()
        {
            _pauseManager.PauseGameProcess();
            int currentScore = _scoreCounter.GetCurrentScore();

            if (currentScore > _currentBestScore)
            {
                await _storedDataHandler.SaveScoreAsync(currentScore);
            }
            else
            {
                OnPlayerDead?.Invoke();
            }
        }

        private void PlayerDeath() => OnPlayerDead?.Invoke();

        public void RequestReloadGame() => _adService.ShowInterstitialAd(ReloadSessionAfterAd);

        public void RequestExitGame() => _sceneService.ExitGame();

        public void RequestRewardedAd()
        {
            if(_storedDataHandler.HasUsedRevive)
            {
                return;
            }

            _storedDataHandler.UseRevive();
            _adService.ShowRewardedAd(ContinueSessionAfterAd);
        }

        private void ReachNewRecord() => OnNewRecord?.Invoke();

        private void ReloadSessionAfterAd() => _sceneService.StartGame();

        private void ContinueSessionAfterAd()
        {
            _pauseManager.PauseGameProcess();
            OnRevive?.Invoke();
            _storedDataHandler.UseRevive();
            Debug.Log("Player revived after rewarded ad!");
        }
        
        public void SaveScoreToLocal() => _storedDataHandler.SaveToLocal();

        public void SaveScoreToCloud() => _storedDataHandler.SaveToCloudAsync().Forget();

        public int MaxShots => _maxShots;

        public void UpdateUI(float time, int currentLaserShots, float currentSpeed, Vector3 position, float rotation)
        {
            OnRechargeTimerUpdated?.Invoke(time);
            _currentShots = currentLaserShots;
            OnCurrentShotsUpdated?.Invoke(_currentShots);
            OnSpeedUpdated?.Invoke(currentSpeed);
            OnCoordinatesUpdated?.Invoke(position, rotation);
        }
    }
}
