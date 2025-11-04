using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids
{
    public class HUDView : MonoBehaviour
    {
        private const int DECIMAL_PLACES_RECHARGE_TIMER = 1;
        private const int DECIMAL_PLACES_COORDINATES = 1;
        private const int DECIMAL_PLACES_ANGLE = 1;
        private const int DECIMAL_PLACES_SPEED = 1;

        public event Action OnRetryButtonClicked;
        public event Action OnExitButtonClicked;
        public event Action OnRewardedButtonClicked;

        [SerializeField] private GameObject _gameOverMenu;
        [SerializeField] private TextMeshProUGUI _coordinatesText;
        [SerializeField] private TextMeshProUGUI _angleText;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _laserShotsText;
        [SerializeField] private TextMeshProUGUI _rechargeTimerText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _rewardedAdsButton;

        public void OnEnable()
        {
            _rewardedAdsButton.onClick.AddListener(() => OnRewardedButtonClicked?.Invoke());
            _retryButton.onClick.AddListener(() => OnRetryButtonClicked?.Invoke());
            _quitButton.onClick.AddListener(() => OnExitButtonClicked?.Invoke());
        }

        public void OnDestroy()
        {
            _rewardedAdsButton.onClick.RemoveListener(() => OnRewardedButtonClicked?.Invoke());
            _retryButton.onClick.RemoveListener(() => OnRetryButtonClicked?.Invoke());
            _quitButton.onClick.RemoveListener(() => OnExitButtonClicked?.Invoke());
        }

        public void UpdateCoordinates(Vector2 playerCoordinates, float angleRotation)
        {
            _coordinatesText.text = $"{System.Math.Round(playerCoordinates.x, DECIMAL_PLACES_COORDINATES)} / {System.Math.Round(playerCoordinates.y, DECIMAL_PLACES_COORDINATES)}";
            _angleText.text = $"{System.Math.Round(angleRotation, DECIMAL_PLACES_ANGLE)}";
        }

        public void UpdateSpeed(float speed)
        {
            _speedText.text = $"{System.Math.Round(speed, DECIMAL_PLACES_SPEED)}";
        }

        public void UpdateScore(int num)
        {
            _scoreText.text = $"{num}";
        }

        public void UpdateCurrentShots(int currentShots, int maxShots)
        {
            _laserShotsText.text = $"{currentShots} / {maxShots}";
        }

        public void UpdateRechargeTimer(float time)
        {
            _rechargeTimerText.text = $"{System.Math.Round(time, DECIMAL_PLACES_RECHARGE_TIMER)}";
        }

        public void ShowGameOverMenu()
        {
            _gameOverMenu.SetActive(true);
        }

        public void HideGameOverMenu()
        {
            _gameOverMenu.SetActive(false);
        }

        public void HideRewardButton()
        {
            _rewardedAdsButton.gameObject.SetActive(false);
        }
    }
}
