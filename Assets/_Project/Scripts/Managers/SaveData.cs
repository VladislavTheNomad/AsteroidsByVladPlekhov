using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class SaveData
    {
        private ScoreCounter _scoreCounter;

        public bool HasUsedRevive { get; private set;}

[Inject]
        public void Construct(ScoreCounter sc)
        { 
            _scoreCounter = sc;
            HasUsedRevive = false;
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

        public void UseRevive() => HasUsedRevive = true;
    }
}
