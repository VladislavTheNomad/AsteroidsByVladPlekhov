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

        private int _maxShots;
        private int _currentShots;
        private SceneService _sceneService;
        private ScoreCounter _scoreCounter;
        private PauseManager _pauseManager;

        [Inject]
        public HUDModel(SceneService ss, ScoreCounter sc, PauseManager pm)
        {
            _sceneService = ss;
            _scoreCounter = sc;
            _pauseManager = pm;

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
            _scoreCounter.SaveScore();
            _pauseManager.PauseGame();
        }

        public void RequestReloadGame()
        {
            _sceneService.ReloadGame();
        }

        public void RequestExitGame()
        {
            _sceneService.ExitGame();
        }

        public void Dispose()
        {
            _scoreCounter.OnScoreChanged -= UpdateScore;
        }

        public int MaxShots => _maxShots;
        public int CurrentShots => _currentShots;
    }
}
