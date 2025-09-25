using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class AsteroidPresenter : MonoBehaviour, IGetPointsOnDestroy, IHaveDeathConditions
    {
        public event Action<int> OnDeathTakeScore;
        public event Action<AsteroidPresenter> OnDeath;

        [SerializeField] private AsteroidView _view;

        private AsteroidModel _model;
        private float _acceleration = 1f;
        private bool _initialized;

        public int AsteroidCurrentSizeLevel { private set; get; }

        [Inject]
        public void Construct(AsteroidModel model)
        {
            _model = model;
        }

        private void OnEnable()
        {
            if (_initialized)
            {
                SetupMovement();
            }
        }

        public void Initialize()
        {
            _view.OnDeath += DestroyAsteroid;
            _view.OnMovement += CheckBounds;
            _view.Initialize();
            _initialized = true;
            SetupMovement();
        }

        public void SetStartConditions()
        {
            _model.CalculateStartPosition(out int startSizeLevel, out Vector2 startSpawnPosition);
            AsteroidCurrentSizeLevel = startSizeLevel;
            _view.SetNewPosition(startSpawnPosition);
            _view.SetScale(Vector3.one);
            _view.SetActive();
        }

        public void CheckBounds()
        {
            Vector3 newPosition = _model.CheckBounds(_view.Transform);
            if(newPosition != Vector3.zero)
            {
                _view.SetNewPosition(newPosition);
            }
        }

        public void GetSmaller(int parentSizeLevel, Transform parentTransform)
        {
            SetCurrentSizeLevel(parentSizeLevel - 1);
            _model.GetSmaller(parentTransform.localScale, _acceleration, out float newAcceleration, out Vector3 newScale);
            _view.SetScale(newScale);
            _view.SetNewPosition(parentTransform.position);
            _view.SetActive();
        }

        public void DestroyAsteroid()
        {
            if (AsteroidCurrentSizeLevel > 1)
            {
                _model.MakeSmallerAsteroids(AsteroidCurrentSizeLevel, _view.Transform);
                HandleDeath();
            }
            else
            {
                HandleDeath();
            }
        }

        public void SetCurrentSizeLevel(int level)
        {
            AsteroidCurrentSizeLevel = level;
        }

        private void SetupMovement()
        {
            _model.SetMovingDestination(_view.Transform, out float startImpulse, out Vector3 destinationPoint);
            _view.Move(destinationPoint, startImpulse, _acceleration);
        }

        public void HandleDeath()
        {
            OnDeathTakeScore?.Invoke(_model.ScorePoints);
            OnDeath?.Invoke(this);
            AsteroidCurrentSizeLevel = _model.MaxSizeLevel;
        }
    }
}