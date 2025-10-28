using System;
using UnityEngine;

namespace Asteroids
{
    public class ScoreCounter
    {
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

        public int GetCurrentScore() => _score;

        private void Update(int score)
        {
            _score += score;
            OnScoreChanged?.Invoke(_score);
        }
    }
}
