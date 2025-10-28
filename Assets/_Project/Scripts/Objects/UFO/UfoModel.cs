using System;
using UnityEngine;

namespace Asteroids
{
    public class UfoModel : IDisposable
    {
        public event Action<bool> IsGamePaused;

        public int ScorePoints { get; private set; }
        public int MoveSpeed { get; private set; }
        public int GapBetweenPositionChanging { get; private set; }

        private PlayerView _playerView;
        private PauseGame _pauseManager;

        public UfoModel(UFOConfig config, PlayerView pv, PauseGame pm) 
        {
            _playerView = pv;
            _pauseManager = pm;

            ScorePoints = config.ScorePoints;
            MoveSpeed = config.MoveSpeed;
            GapBetweenPositionChanging = config.GapBetweenPositionChanging;

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