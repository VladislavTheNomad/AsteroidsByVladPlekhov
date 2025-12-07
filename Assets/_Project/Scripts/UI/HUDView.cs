using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        public event Action OnSaveToLocalClicked;
        public event Action OnSaveToCloudClicked;

        private UnityAction _onRewardAction;
        private UnityAction _onRetryAction;
        private UnityAction _onQuitAction;
        private UnityAction _onSaveToLocalAction;
        private UnityAction _onSaveToCloudAction;

        [SerializeField] private GameObject _gameOverMenu;
        [SerializeField] private GameObject _newRecordMenu;
        [SerializeField] private TextMeshProUGUI _coordinatesText;
        [SerializeField] private TextMeshProUGUI _angleText;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _laserShotsText;
        [SerializeField] private TextMeshProUGUI _rechargeTimerText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _rewardedAdsButton;
        [SerializeField] private Button _saveToLocalButton;
        [SerializeField] private Button _saveToCloudButton;

        public void Awake()
        {
            _onRewardAction = () => OnRewardedButtonClicked?.Invoke();
            _onRetryAction = () => OnRetryButtonClicked?.Invoke();
            _onQuitAction = () => OnExitButtonClicked?.Invoke();
            _onSaveToLocalAction = () => OnSaveToLocalClicked?.Invoke();
            _onSaveToCloudAction = () => OnSaveToCloudClicked?.Invoke();
        }


        public void OnEnable()
        {
            _rewardedAdsButton.onClick.AddListener(_onRewardAction);
            _retryButton.onClick.AddListener(_onRetryAction);
            _quitButton.onClick.AddListener(_onQuitAction);
            _saveToLocalButton.onClick.AddListener(_onSaveToLocalAction);
            _saveToCloudButton.onClick.AddListener(_onSaveToCloudAction);
        }

        public void OnDestroy()
        {
            _rewardedAdsButton.onClick.RemoveListener(_onRewardAction);
            _retryButton.onClick.RemoveListener(_onRetryAction);
            _quitButton.onClick.RemoveListener(_onQuitAction);
            _saveToLocalButton.onClick.RemoveListener(_onSaveToLocalAction);
            _saveToCloudButton.onClick.RemoveListener(_onSaveToCloudAction);
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

        public void UpdateBestScore(int num)
        {
            _bestScoreText.text = $"{num}";
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

        public void ShowNewRecordUI()
        {
           _newRecordMenu.SetActive(true);
        }

        public void HideNewRecordUI()
        {
            _newRecordMenu.SetActive(false);
        }

    }
}
