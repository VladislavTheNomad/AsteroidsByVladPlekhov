using UnityEngine;

namespace Asteroids
{
    public class UfoModel
    {
        public int ScorePoints { get; private set; }
        public int MoveSpeed { get; private set; }
        public int GapBetweenPositionChanging { get; private set; }

        public UfoModel(UFOConfig config) 
        {
            ScorePoints = config.ScorePoints;
            MoveSpeed = config.MoveSpeed;
            GapBetweenPositionChanging = config.GapBetweenPositionChanging;
        }
    }
}
