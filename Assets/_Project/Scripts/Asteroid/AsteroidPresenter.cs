using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class AsteroidPresenter : IGetPointsOnDestroy, IDisposable
    {
        public event Action<int> OnDeathTakeScore;
        public event Action OnDeath;

        private AsteroidView _view;
        private AsteroidModel _model;
        private float _acceleration = 1f;
        private bool _initialized;

        public int AsteroidCurrentSizeLevel { private set; get; }

        [Inject]
        public void Construct(AsteroidModel model)
        {
            _model = model;
        }

        public void Initialize(AsteroidView newView)
        {
            _view = newView;
            _view.OnDeath += DestroyAsteroid;
            _view.OnMovement += CheckBounds;
            _view.OnSetNew += SetStartConditions;
            _view.OnEnabled += Starter;
            _view.OnGetSmaller += GetSmaller;
            _view.Initialize();
            _initialized = true;
            Starter();
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
            AsteroidCurrentSizeLevel = parentSizeLevel - 1;
            _model.GetSmaller(parentTransform, _acceleration, out float newAcceleration, out Vector3 newScale);
            _acceleration = newAcceleration;
            _view.SetScale(newScale);
            _view.SetNewPosition(_view.Transform.position);
            _view.SetActive();
        }

        public void DestroyAsteroid()
        {
            if (AsteroidCurrentSizeLevel > 1)
            {
                _model.MakeSmallerAsteroids(AsteroidCurrentSizeLevel, _view.Transform);
            }
            HandleDeath();
        }

        public void SetCurrentSizeLevel(int level)
        {
            AsteroidCurrentSizeLevel = level;
        }

        public void HandleDeath()
        {
            OnDeathTakeScore?.Invoke(_model.ScorePoints);
            OnDeath?.Invoke();
            AsteroidCurrentSizeLevel = _model.MaxSizeLevel;
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.OnDeath -= DestroyAsteroid;
                _view.OnMovement -= CheckBounds;
                _view.OnSetNew -= SetStartConditions;
                _view.OnEnabled -= Starter;
                _view.OnGetSmaller -= GetSmaller;
            }
        }

        private void Starter()
        {
            if (_initialized)
            {
                SetupMovement();
            }
        }

        private void SetupMovement()
        {
            _model.SetMovingDestination(_view.Transform, out float startImpulse, out Vector3 destinationPoint);
            _view.Move(destinationPoint, startImpulse, _acceleration);
        }
    }
}