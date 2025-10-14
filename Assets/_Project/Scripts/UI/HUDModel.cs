using System;
using UnityEngine;

namespace Asteroids
{
    public class HUDModel
    {
        public event Action<Vector2, float> OnCoordinatesUpdated;
        public event Action<float> OnSpeedUpdated;
        public event Action<int> OnCurrentShotsUpdated;
        public event Action<float> OnRechargeTimerUpdated;
        public event Action OnPlayerDead;

        private int _maxShots;
        private int _currentShots;

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

        public void PlayerDied()
        {
            OnPlayerDead?.Invoke();
        }

        public int MaxShots => _maxShots;
        public int CurrentShots => _currentShots;
    }
}
