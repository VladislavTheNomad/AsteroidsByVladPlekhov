using UnityEngine;

namespace Asteroids
{
    public class AsteroidModel
    {
        private const float HEIGHT_BUFFER = 0.2f;
        private const float WIDTH_BUFFER = 0.3f;
        private const float SCREEN_RIGHT_BOUND = 1f;
        private const float SCREEN_LEFT_BOUND = 0f;
        private const float SCREEN_TOP_BOUND = 1f;
        private const float SCREEN_BOTTOM_BOUND = 0f;
        private const float SCALE_REDUCE = 0.5f;

        public float MaxMoveSpeed { get; private set; }
        public float MinMoveSpeed { get; private set; }
        public float AccelerationModificator { get; private set; }
        public int MaxSizeLevel { get; private set; }
        public int SmallAsteroidQuantity { get; private set; }
        public int ScorePoints { get; private set; }

        private Camera _mainCamera;
        private UtilsCalculatePositions _utils;
        private AsteroidFactory _factory;
        private Vector3 _bottomLeft;
        private Vector3 _topRight;

        public AsteroidModel(AsteroidConfig asteroidConfig, Camera camera, UtilsCalculatePositions utilsCalculatePositions, AsteroidFactory factory)
        {
            MaxMoveSpeed = asteroidConfig.MaxMoveSpeed;
            MinMoveSpeed = asteroidConfig.MinMoveSpeed;
            AccelerationModificator = asteroidConfig.AccelerationModificator;
            MaxSizeLevel = asteroidConfig.MaxSizeLevel;
            SmallAsteroidQuantity = asteroidConfig.SmallAsteroidQuantity;
            ScorePoints = asteroidConfig.ScorePoints;

            _mainCamera = camera;
            _utils = utilsCalculatePositions;
            _factory = factory;

            _bottomLeft = _mainCamera.ViewportToWorldPoint(Vector2.zero);
            _topRight = _mainCamera.ViewportToWorldPoint(Vector2.one);
        }

        public Vector3 CheckBounds(Transform transform)
        {
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
            bool isOutOfBounds =
        viewportPos.x < SCREEN_LEFT_BOUND - HEIGHT_BUFFER ||
        viewportPos.x > SCREEN_RIGHT_BOUND + HEIGHT_BUFFER ||
        viewportPos.y > SCREEN_TOP_BOUND + WIDTH_BUFFER ||
        viewportPos.y < SCREEN_BOTTOM_BOUND - WIDTH_BUFFER;


            if (isOutOfBounds)
            {
                Vector3 newPosition = transform.position;

                if (viewportPos.x > SCREEN_RIGHT_BOUND)
                {
                    newPosition.x = _bottomLeft.x;
                }
                else if (viewportPos.x < SCREEN_LEFT_BOUND)
                {
                    newPosition.x = _topRight.x;
                }

                if (viewportPos.y > SCREEN_TOP_BOUND)
                {
                    newPosition.y = _bottomLeft.y;
                }
                else if (viewportPos.y < SCREEN_BOTTOM_BOUND)
                {
                    newPosition.y = _topRight.y;
                }
                return newPosition;
            }
            return Vector3.zero;
        }

        public void CalculateStartPosition(out int sizeLevelAtStart, out Vector2 spawnPosition)
        {
            sizeLevelAtStart = MaxSizeLevel;
            spawnPosition = _utils.GetRandomSpawnPosition();
        }

        public void GetSmaller(Transform parentTransform, float currentAcceleration, out float newAcceleration, out Vector3 newScale)
        {
            newAcceleration = AddAcceleration(currentAcceleration);
            newScale = parentTransform.localScale;
            newScale.x *= SCALE_REDUCE;
            newScale.y *= SCALE_REDUCE;
        }

        public void MakeSmallerAsteroids(int parentSizeLevel, Transform parentTransform)
        {
            for (int i = 0; i < SmallAsteroidQuantity; i++)
            {
                AsteroidView smallAsteroid = _factory.GetAsteroidFromPool();
                smallAsteroid.transform.position = parentTransform.position;
                smallAsteroid.GetSmaller(parentSizeLevel, parentTransform);
            }
        }

        public void SetMovingDestination(Transform transform, out float startImpulse, out Vector3 destinationPoint)
        {
            Vector3 targetPoint = new Vector3(Random.Range(_bottomLeft.x, _topRight.x), Random.Range(_bottomLeft.y, _topRight.y), 0);
            destinationPoint = (targetPoint - transform.position).normalized;
            startImpulse = Random.Range(MinMoveSpeed, MaxMoveSpeed);
        }

        private float AddAcceleration(float currentAcceleration)
        {
            return currentAcceleration + AccelerationModificator;
        }
    }
}