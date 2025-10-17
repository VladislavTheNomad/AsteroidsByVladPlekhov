using System;
using UnityEngine;

namespace Asteroids
{
    public class ScoreCounter
    {
        public class SaveData
        {
            public int _finalScore;

            public SaveData(int score)
            {
                _finalScore = score;
            }
        }

        private const int INITIAL_SCORE = 0;

        public event Action<int> OnScoreChanged;

        private int _score = INITIAL_SCORE;

        public void SubscribeOnDeath(IGetPointsOnDestroy enemy)
        {
            enemy.OnDeathTakeScore += Update;
        }

        public void UnsubscribeOnDeath(IGetPointsOnDestroy enemy)
        {
            enemy.OnDeathTakeScore -= Update;
        }

        public void SaveScore()
        {
            SaveData saveData = new SaveData(_score);
            if (PlayerPrefs.HasKey("BestScore"))
            {
                int currentBestScore = PlayerPrefs.GetInt("BestScore");

                if (currentBestScore >= saveData._finalScore) return;

                currentBestScore = saveData._finalScore;
                PlayerPrefs.SetInt("BestScore", currentBestScore);
            }
            else
            {
                PlayerPrefs.SetInt("BestScore", saveData._finalScore);
            }
        }

        private void Update(int score)
        {
            _score += score;
            OnScoreChanged?.Invoke(_score);
        }
    }
}
