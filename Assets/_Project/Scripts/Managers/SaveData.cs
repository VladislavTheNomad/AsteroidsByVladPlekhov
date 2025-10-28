using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class SaveData
    {
        private ScoreCounter _scoreCounter;

        [Inject]
        public void Construct(ScoreCounter sc)
        { 
            _scoreCounter = sc;
        }

        public void SaveScore()
        {
            int currentScore = _scoreCounter.GetCurrentScore();

            if (PlayerPrefs.HasKey("BestScore"))
            {
                int currentBestScore = PlayerPrefs.GetInt("BestScore");

                if (currentBestScore >= currentScore) return;

                currentBestScore = currentScore;
                PlayerPrefs.SetInt("BestScore", currentBestScore);
            }
            else
            {
                PlayerPrefs.SetInt("BestScore", currentScore);
            }
        }



    }
}
