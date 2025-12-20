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
        public HUDModel(SceneService ss, ScoreCounter sc, PauseGame pm, StoredDataHandler sd, IAdService adService)
        {
            _sceneService = ss;
            _scoreCounter = sc;
            _storedDataHandler = sd;
            _pauseManager = pm;
            _adService = adService;

            _scoreCounter.OnScoreChanged += UpdateScore;
            _storedDataHandler.OnChoosePlaceToSave += ReachNewRecord;
            _storedDataHandler.OnSaveScoreAfterDeath += PlayerDeath;
        }        

        public void Initialize()
        {
            DownloadBestScore().Forget();
        }

        private async UniTaskVoid DownloadBestScore()
        {
            _currentBestScore = await _storedDataHandler.GetBestScoreAndUpdateData();
            OnBestScoreSetup?.Invoke(_currentBestScore);
        }

        public void Dispose()
        {
            _scoreCounter.OnScoreChanged -= UpdateScore;
            _storedDataHandler.OnChoosePlaceToSave -= ReachNewRecord;
            _storedDataHandler.OnSaveScoreAfterDeath -= PlayerDeath;
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
                PlayerDeath();
            }
        }

        private void PlayerDeath()
        {
            OnPlayerDead?.Invoke();
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
            if(_storedDataHandler.HasUsedRevive == true)
            {
                return;
            }

            _storedDataHandler.UseRevive();
            _adService.ShowRewardedAd(ContinueSessionAfterAd);
        }

        public void ReachNewRecord()
        {
            OnNewRecord?.Invoke();
        }

        private void ReloadSessionAfterAd()
        {
            _sceneService.StartGame();
        }

        private void ContinueSessionAfterAd()
        {
            _pauseManager.PauseGameProcess();
            OnRevive?.Invoke();
            _storedDataHandler.UseRevive();
            Debug.Log("Player revived after rewarded ad!");
        }

        
        public void SaveScoreToLocal()
        {
            _storedDataHandler.SaveToLocal();
        }

        public void SaveScoreToCloud()
        {
            _storedDataHandler.SaveToCloudAsync();
        }

        public int MaxShots => _maxShots;
        public int CurrentShots => _currentShots;
    }
}
