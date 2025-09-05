using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Asteroids
{
    public class UIManager : MonoBehaviour, IInitiable
    {
        private const int INITIAL_SCORE = 0;
        private const float TIME_SCALE_PAUSED = 0f;
        private const float TIMER_THRESHOLD = 0f;
        private const int DECIMAL_PLACES_RECHARGE_TIMER = 2;
        private const string DEFAULT_RECHARGE_TIMER_TEXT = "0";
        private const int DECIMAL_PLACES_COORDINATES = 2;
        private const int DECIMAL_PLACES_ANGLE = 1;
        private const int DECIMAL_PLACES_SPEED = 1;

        [SerializeField] private CheckDeathConditions _playerDeadConditions;
        [SerializeField] private FireLaser _playerLaser;
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

        private void OnDisable()
        {
            _retryButton.onClick.RemoveListener(RetryButtonClick);
            _quitButton.onClick.RemoveListener(QuitButtonClick);

            _playerDeadConditions.OnPlayerIsDead -= PlayerIsDead;
            _playerLaser.OnAmountLaserShotChange -= UpdateCurrentShot;
            _playerLaser.OnRechargeTimer -= UpdateRechargeTimer;
        }

        public void Installation()
        {
            _playerDeadConditions.OnPlayerIsDead += PlayerIsDead;
            _playerLaser.OnAmountLaserShotChange += UpdateCurrentShot;
            _playerLaser.OnRechargeTimer += UpdateRechargeTimer;

            UpdateCurrentShot();

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

        private void PlayerIsDead()
        {
            _gameOverMenu.SetActive(true);
            Time.timeScale = TIME_SCALE_PAUSED;
        }

        private void UpdateRechargeTimer()
        {
            float[] timers = _playerLaser.CurrentRechargeTimers.Where(t => t > TIMER_THRESHOLD).ToArray();
            if (timers.Length > 0)
            {
                float minRechargeCooldown = timers.Min();
                _rechargeTimerText.text = $"{System.Math.Round(minRechargeCooldown, DECIMAL_PLACES_RECHARGE_TIMER)}";
            }
            else
            {
                _rechargeTimerText.text = DEFAULT_RECHARGE_TIMER_TEXT;
            }
        }

        private void UpdateCurrentShot()
        {
            _laserShotsText.text = $"{_playerLaser.CurrentLaserShoots} / {_playerLaser.GetMaxShots()}";
        }

        private void RetryButtonClick()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("MainScene");
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
