using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Asteroids
{
    public class GameHUDManager : MonoBehaviour, IInitializable
    {
        private const int INITIAL_SCORE = 0;
        private const int DECIMAL_PLACES_RECHARGE_TIMER = 2;
        private const int DECIMAL_PLACES_COORDINATES = 2;
        private const int DECIMAL_PLACES_ANGLE = 1;
        private const int DECIMAL_PLACES_SPEED = 1;

        public event Action OnGamePaused;

        [SerializeField] private GameObject _gameOverMenu;
        [SerializeField] private TextMeshProUGUI _coordinatesText;
        [SerializeField] private TextMeshProUGUI _angleText;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _laserShotsText;
        [SerializeField] private TextMeshProUGUI _rechargeTimerText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;

        private int _score = INITIAL_SCORE;
        private int _maxShots;
        private PlayerPresenter _playerPresenter;

        [Inject]
        public void Constructor(PlayerPresenter pp)
        {
            _playerPresenter = pp;
        }

        private void OnDisable()
        {
            _retryButton.onClick.RemoveListener(RetryButtonClick);
            _quitButton.onClick.RemoveListener(QuitButtonClick);

            _playerPresenter.OnPlayerIsDead -= PlayerIsDead;
            _playerPresenter.OnRechargeTimer -= UpdateRechargeTimer;
        }

        public void Initialize()
        {
            _playerPresenter.OnPlayerIsDead += PlayerIsDead;
            _playerPresenter.OnRechargeTimer += UpdateRechargeTimer;

            _retryButton.onClick.AddListener(RetryButtonClick);
            _quitButton.onClick.AddListener(QuitButtonClick);
        }

        public void SubscribeOnDeath(IGetPointsOnDestroy enemy)
        {
            enemy.OnDeathTakeScore += UpdateScore;
        }

        public void UnsubscribeOnDeath(IGetPointsOnDestroy enemy)
        {
            enemy.OnDeathTakeScore -= UpdateScore;
        }

        public void SetMaxLaserShots(int maxShots)
        {
            _maxShots = maxShots;
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
            _score += num;
            _scoreText.text = $"{_score}";
        }

        public void UpdateCurrentShots(int currentShots)
        {
            _laserShotsText.text = $"{currentShots} / {_maxShots}";
        }

        private void PlayerIsDead()
        {
            _gameOverMenu.SetActive(true);
            OnGamePaused?.Invoke();
        }

        private void UpdateRechargeTimer(float time)
        {
            _rechargeTimerText.text = $"{System.Math.Round(time, DECIMAL_PLACES_RECHARGE_TIMER)}";
        }

        private void RetryButtonClick()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        private void QuitButtonClick()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}