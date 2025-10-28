using System;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidModel
    {
        private const float SCALE_REDUCE = 0.5f;

        public event Action<bool> IsGamePaused;

        public float MaxMoveSpeed { get; private set; }
        public float MinMoveSpeed { get; private set; }
        public float AccelerationModificator { get; private set; }
        public int MaxSizeLevel { get; private set; }
        public int SmallAsteroidQuantity { get; private set; }
        public int ScorePoints { get; private set; }

        private Camera _mainCamera;
        private UtilsCalculatePositions _utils;
        private AsteroidFactory _factory;
        private PauseGame _pauseManager;
        private Vector3 _bottomLeft;
        private Vector3 _topRight;

        public AsteroidModel(AsteroidConfig asteroidConfig, Camera camera, UtilsCalculatePositions utils, AsteroidFactory factory, PauseGame pm)
        {
            MaxMoveSpeed = asteroidConfig.MaxMoveSpeed;
            MinMoveSpeed = asteroidConfig.MinMoveSpeed;
            AccelerationModificator = asteroidConfig.AccelerationModificator;
            MaxSizeLevel = asteroidConfig.MaxSizeLevel;
            SmallAsteroidQuantity = asteroidConfig.SmallAsteroidQuantity;
            ScorePoints = asteroidConfig.ScorePoints;

            _mainCamera = camera;
            _utils = utils;
            _factory = factory;
            _pauseManager = pm;

            _bottomLeft = _mainCamera.ViewportToWorldPoint(Vector2.zero);
            _topRight = _mainCamera.ViewportToWorldPoint(Vector2.one);

            _pauseManager.GameIsPaused += TogglePause;
        }

        public Vector3 CheckBounds(Transform transform)
        {
            return _utils.CheckBounds(transform);
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
            Vector3 targetPoint = new Vector3(UnityEngine.Random.Range(_bottomLeft.x, _topRight.x), UnityEngine.Random.Range(_bottomLeft.y, _topRight.y), 0);
            destinationPoint = (targetPoint - transform.position).normalized;
            startImpulse = UnityEngine.Random.Range(MinMoveSpeed, MaxMoveSpeed);
        }

        private float AddAcceleration(float currentAcceleration)
        {
            return currentAcceleration + AccelerationModificator;
        }

        private void TogglePause(bool condition)
        {
            switch (condition)
            {
                case true:
                    IsGamePaused?.Invoke(true);
                    break;
                case false:
                    IsGamePaused?.Invoke(false);
                    break;
            }
        }
    }
}