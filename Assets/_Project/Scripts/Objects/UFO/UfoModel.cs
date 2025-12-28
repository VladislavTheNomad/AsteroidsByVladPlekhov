using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class UfoModel : IInitializable, IDisposable
    {
        public event Action<bool> IsGamePaused;

        public int ScorePoints { get; private set; }
        public float MoveSpeed { get; private set; }
        public float GapBetweenPositionChanging { get; private set; }

        private PlayerView _playerView;
        private PauseGame _pauseManager;

        public UfoModel(RemoteConfigService configService, PlayerView playerView, PauseGame pauseManager) 
        {
            _playerView = playerView;
            _pauseManager = pauseManager;

            ScorePoints = configService.Config.UFOScorePoints;
            MoveSpeed = configService.Config.UFOMoveSpeed;
            GapBetweenPositionChanging = configService.Config.GapBetweenPositionChanging;
        }
        
        public void Initialize()
        {
            _pauseManager.GameIsPaused += TogglePause;
        }

        public void Dispose()
        {
            _pauseManager.GameIsPaused -= TogglePause;
        }

        public void GetNewDestination(Transform transform, out Vector3 destination, out float moveSpeed)
        {
            destination = (_playerView.transform.position - transform.position).normalized;
            moveSpeed = MoveSpeed;
        }

        public bool CheckNull(Transform transform)
        {
            if (transform == null || _playerView == null || !_playerView.gameObject.activeSelf)
            {
                return false;
            }
            return true;
        }

        private void TogglePause(bool condition)
        {
            switch (condition)
            {
                case true:
                    IsGamePaused?.Invoke(true);
                    break;
                case false:
                    IsGamePaused?.Invoke(false);
                    break;
            }
        }

      
    }
}