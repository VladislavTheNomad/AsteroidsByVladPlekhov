namespace Asteroids
{
    public class AsteroidModel
    {
        public float MaxMoveSpeed { get; private set; }
        public float MinMoveSpeed { get; private set; }
        public float AccelerationModificator { get; private set; }
        public int MaxSizeLevel { get; private set; }
        public int SmallAsteroidQuantity { get; private set; }
        public int ScorePoints { get; private set; }

        public AsteroidModel(AsteroidConfig asteroidConfig)
        {
            MaxMoveSpeed = asteroidConfig.MaxMoveSpeed;
            MinMoveSpeed = asteroidConfig.MinMoveSpeed;
            AccelerationModificator = asteroidConfig.AccelerationModificator;
            MaxSizeLevel = asteroidConfig.MaxSizeLevel;
            SmallAsteroidQuantity = asteroidConfig.SmallAsteroidQuantity;
            ScorePoints = asteroidConfig.ScorePoints;
        }
    }
}