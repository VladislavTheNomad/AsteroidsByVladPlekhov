using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class AsteroidBehaviour : MonoBehaviour, IHaveDeathConditions, IGetPointsOnDestroy
    {
        private const int SCORE_POINTS = 1;
        private const float HEIGHT_BUFFER = 0.2f;
        private const float WIDTH_BUFFER = 0.3f;
        private const float SCREEN_RIGHT_BOUND = 1f;
        private const float SCREEN_LEFT_BOUND = 0f;
        private const float SCREEN_TOP_BOUND = 1f;
        private const float SCREEN_BOTTOM_BOUND = 0f;

        public event Action<int> OnDeathTakeScore;
        public event Action<AsteroidBehaviour> OnDeath;

        [SerializeField] private AsteroidSplitting _splittingScript;
        [SerializeField] private float _maxMoveSpeed;
        [SerializeField] private float _minMoveSpeed;
        [SerializeField] private int _maxSizeLevel = 3;

        private Camera _mainCamera;
        private Vector3 _destination;
        private float _moveImpulse;
        private bool _isInitialized;
        private Rigidbody2D _rb;
        private float _acceleration = 1f;

        public int _asteroidCurrentSizeLevel { private set; get; }

        [Inject]
        public void Construct(Camera camera)
        {
            _mainCamera = camera;
        }

        private void OnEnable()
        {
            if (!_isInitialized) return;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            SetupMovement();
        }

        private void OnDisable()
        {
            _acceleration = 1f;
            SetMaxSizeLevel(_maxSizeLevel);
        }

        private void Update()
        {
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
            if (viewportPos.x < SCREEN_LEFT_BOUND - HEIGHT_BUFFER || viewportPos.x > SCREEN_RIGHT_BOUND + HEIGHT_BUFFER || viewportPos.y > SCREEN_TOP_BOUND + WIDTH_BUFFER || viewportPos.y < SCREEN_BOTTOM_BOUND - WIDTH_BUFFER)
            {
                Vector3 bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
                Vector3 topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
                Vector3 newPosition = transform.position;

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
                _rb.MovePosition(newPosition);
            }
        }

        public void Initialize(PoolManager<AsteroidBehaviour> manager, UtilsCalculatePositions calculateUtils)
        {
            _rb = GetComponent<Rigidbody2D>();
            _splittingScript.Initialize(manager);
            _isInitialized = true;
        }

        public void DeathConditions()
        {
            OnDeathTakeScore?.Invoke(SCORE_POINTS);
            OnDeath?.Invoke(this);
        }

        public void SetCurrentSizeLevel(int level)
        {
            _asteroidCurrentSizeLevel = level;
        }

        public void SetMaxSizeLevel(int level)
        {
            _maxSizeLevel = level;
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
            _destination = (targetPoint - transform.position).normalized;
            _moveImpulse = UnityEngine.Random.Range(_minMoveSpeed, _maxMoveSpeed);
            _rb.AddForce(_destination * _moveImpulse * _acceleration, ForceMode2D.Force);
        }
    }
}