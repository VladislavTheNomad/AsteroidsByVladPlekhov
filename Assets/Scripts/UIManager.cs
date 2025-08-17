using System.Linq;
using TMPro;
using UnityEngine;

namespace Asteroids
{
    public class UIManager : MonoBehaviour, IInitiable
    {
        //own
        public int sortingIndex => 999;
        private int score = 0;

        //connections
        [SerializeField] private DeathConditions playerDeadConditions;
        [SerializeField] private FireLaser playerLaser;

        //connections with UI components
        [SerializeField] private GameObject gameOverMenu;
        [SerializeField] private TextMeshProUGUI coordinatesText;
        [SerializeField] private TextMeshProUGUI angleText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI laserShotsText;
        [SerializeField] private TextMeshProUGUI RechargeTimerText;
        [SerializeField] private TextMeshProUGUI ScoreText;

        public void Installation()
        {
            playerDeadConditions.OnPlayerIsDead += PlayerIsDead;
            playerLaser.OnAmountLaserShotChange += UpdateCurrentShot;
            playerLaser.OnRechangeTimer += UpdateRechargeTimer;
            UpdateCurrentShot();
        }

        private void PlayerIsDead()
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        private void UpdateRechargeTimer()
        {
            float[] timers = playerLaser.currentRechageTimers.Where(t => t > 0f).ToArray();
            if (timers.Length > 0)
            {
                float minRechargeCooldown = timers.Min();
                RechargeTimerText.text = $"{System.Math.Round(minRechargeCooldown, 2)}";
            }
            else
            {
                RechargeTimerText.text = "0";
            }
        }

        private void UpdateCurrentShot()
        {
            laserShotsText.text = $"{playerLaser.currentLaserShoots} / {playerLaser.GetMaxShots()}";
        }

        public void UpdateCoordinates(Vector2 playerCoordinates, float angleRotation)
        {
            coordinatesText.text = $"{System.Math.Round(playerCoordinates.x, 2)} / {System.Math.Round(playerCoordinates.y, 2)}";
            angleText.text = $"{System.Math.Round(angleRotation, 1)}";
        }

        public void UpdateSpeed(float speed)
        {
            speedText.text = $"{System.Math.Round(speed, 1)}";
        }

        public void UpdateScore(int num)
        {
            score += num;
            ScoreText.text = $"{score}" ;
        }
    }
}
