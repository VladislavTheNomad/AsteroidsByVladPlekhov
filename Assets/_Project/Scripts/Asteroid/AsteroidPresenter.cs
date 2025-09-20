using System;
using UnityEditor.EditorTools;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class AsteroidPresenter : MonoBehaviour, IGetPointsOnDestroy, IHaveDeathConditions
    {
        private const float HEIGHT_BUFFER = 0.2f;
        private const float WIDTH_BUFFER = 0.3f;
        private const float SCREEN_RIGHT_BOUND = 1f;
        private const float SCREEN_LEFT_BOUND = 0f;
        private const float SCREEN_TOP_BOUND = 1f;
        private const float SCREEN_BOTTOM_BOUND = 0f;
        private const float SCALE_REDUCE = 0.5f;

        public event Action<int> OnDeathTakeScore;
        public event Action<AsteroidPresenter> OnDeath;

        [SerializeField] private AsteroidView _view;

        private AsteroidModel _model;
        private GamePoolsController _poolManager;
        private UtilsCalculatePositions _calculatePositions;
        private Camera _mainCamera;
        private float _acceleration = 1f;
        private bool _initialized;

        public int _asteroidCurrentSizeLevel { private set; get; }

        [Inject]
        public void Construct(Camera camera, AsteroidModel model, GamePoolsController poolsController, UtilsCalculatePositions utils)
        {
            _mainCamera = camera;
            _model = model;
            _poolManager = poolsController;
            _calculatePositions = utils;
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
            _asteroidCurrentSizeLevel = _model.MaxSizeLevel;
            Vector2 spawnPosition = _calculatePositions.GetRandomSpawnPosition();
            _view.SetNewPosition(spawnPosition);
            _view.SetScale(Vector3.one);
            _view.SetActive();
        }

        public void CheckBounds()
        {
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(_view.transform.position);
            if (viewportPos.x < SCREEN_LEFT_BOUND - HEIGHT_BUFFER || viewportPos.x > SCREEN_RIGHT_BOUND + HEIGHT_BUFFER || viewportPos.y > SCREEN_TOP_BOUND + WIDTH_BUFFER || viewportPos.y < SCREEN_BOTTOM_BOUND - WIDTH_BUFFER)
            {
                Vector3 bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _view.transform.position.z));
                Vector3 topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _view.transform.position.z));
                Vector3 newPosition = _view.transform.position;

                if (viewportPos.x > SCREEN_RIGHT_BOUND)
                {
                    newPosition.x = bottomLeft.x;
                }
                else if (viewportPos.x < SCREEN_LEFT_BOUND)
                {
                    newPosition.x = topRight.x;
                }

                if (viewportPos.y > SCREEN_TOP_BOUND)
                {
                    newPosition.y = bottomLeft.y;
                }
                else if (viewportPos.y < SCREEN_BOTTOM_BOUND)
                {
                    newPosition.y = topRight.y;
                }
                _view.SetNewPosition(newPosition);
            }
        }

        public void GetSmaller(int parentSizeLevel, Vector3 parentPosition, Vector3 parentScale)
        {
            SetCurrentSizeLevel(parentSizeLevel - 1);
            AddAcceleration(_model.AccelerationModificator);
            Vector3 newScale = parentScale;
            newScale.x *= SCALE_REDUCE;
            newScale.y *= SCALE_REDUCE;
            _view.SetScale(newScale);
            _view.SetNewPosition(parentPosition);
            _view.SetActive();
        }

        public void DestroyAsteroid()
        {
            if (_asteroidCurrentSizeLevel > 1)
            {
                for (int i = 0; i < _model.SmallAsteroidQuantity; i++)
                {
                    AsteroidPresenter smallAsteroid = _poolManager.GetAsteroidPool().Get();
                    smallAsteroid.GetSmaller(_asteroidCurrentSizeLevel, _view.transform.position, _view.transform.localScale);
                }
                DeathConditions();
            }
            else
            {
                DeathConditions();
            }
        }

        public void SetCurrentSizeLevel(int level)
        {
            _asteroidCurrentSizeLevel = level;
        }

        public void AddAcceleration(float modificator)
        {
            _acceleration += modificator;
        }

        private void SetupMovement()
        {
            Vector2 bottomLeft = _mainCamera.ViewportToWorldPoint(Vector2.zero);
            Vector2 topRight = _mainCamera.ViewportToWorldPoint(Vector2.one);

            Vector3 targetPoint = new Vector3(UnityEngine.Random.Range(bottomLeft.x, topRight.x), UnityEngine.Random.Range(bottomLeft.y, topRight.y), 0);
            Vector3 destination = (targetPoint - _view.transform.position).normalized;
            float moveImpulse = UnityEngine.Random.Range(_model.MinMoveSpeed, _model.MaxMoveSpeed);
            _view.DoMove(destination, moveImpulse, _acceleration);
        }

        public void DeathConditions()
        {
            OnDeathTakeScore?.Invoke(_model.ScorePoints);
            OnDeath?.Invoke(this);
            _asteroidCurrentSizeLevel = _model.MaxSizeLevel;
        }
    }
}
