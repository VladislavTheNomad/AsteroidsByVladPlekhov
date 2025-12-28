using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    [RequireComponent(typeof(LineRenderer), typeof(Rigidbody2D), typeof(Transform))]
    public class PlayerView : MonoBehaviour, IInitializable, IDisposable
    {
        private const float LASER_DURATION = 0.25f;
        private const float LASER_LENGTH = 20f;
        
        [SerializeField] private ParticleSystem _fireBulletParticles;

        public event Action MoveRequested;
        public event Action<float> RotateRequested;
        public event Action FireBulletRequested;
        public event Action FireLaserRequested;
        public event Action CollisionDetected;
        public event Action OnPauseClick;

        private LineRenderer _lineRenderer;
        private PlayerControls _playerControls;
        private WaitForSeconds _laserVisualLifespan = new WaitForSeconds(LASER_DURATION);

        [field: SerializeField] public GameObject LeftBound { get; private set; }
        [field: SerializeField] public GameObject RightBound { get; private set; }
        public float _moveInput { get; private set; }
        public float _rotateInput { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        public Transform ViewTransform { get; private set; }

        private float _shootBulletInput;
        private bool isPaused = false;
        private Vector2 savedVelocity;
        private float savedAngularVelocity;

        private void OnDisable()
        {
            _playerControls?.Player.Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<AsteroidView>(out AsteroidView av) || collision.TryGetComponent<UfoView>(out UfoView uv))
            {
                CollisionDetected?.Invoke();
            }
        }

        public void Update()
        {
            if (isPaused) return;

            if (_shootBulletInput > 0f)
            {
                FireBulletRequested?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (isPaused) return;

            RotateRequested?.Invoke(_rotateInput);

            if (_moveInput > 0f)
            {
                MoveRequested?.Invoke();
            }
        }

        public void Initialize()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            Rb = GetComponent<Rigidbody2D>();
            ViewTransform = GetComponent<Transform>();

            _playerControls = new PlayerControls();
            RegisterDependencies();
        }

        public void RegisterDependencies()
        {
            if (_playerControls == null) return;
            _playerControls.Player.Enable();
            _playerControls.Player.Move.performed += context => _moveInput = context.ReadValue<float>();
            _playerControls.Player.Move.canceled += context => _moveInput = 0f;
            _playerControls.Player.Rotate.performed += context => _rotateInput = context.ReadValue<float>();
            _playerControls.Player.Rotate.canceled += context => _rotateInput = 0f;

            _playerControls.Player.ShootBullet.performed += context => _shootBulletInput = 1f;
            _playerControls.Player.ShootBullet.canceled += context => _shootBulletInput = 0f;

            _playerControls.Player.ShootLaser.started += context => ShootLaser();

            _playerControls.Player.Pause.started += context => OnPauseClick?.Invoke();
        }

        public void Teleport(Vector3 newPosition)
        {
            Rb.MovePosition(newPosition);
        }

        public void Rotate(float direction, float speed)
        {
            Rb.AddTorque(direction * speed * Time.deltaTime);
        }

        public void Move(float speed, ForceMode2D forceType)
        {
            Rb.AddForce(speed * transform.up, forceType);
        }

        public void TogglePause(bool switcher)
        {
            if (Rb.Equals(null)) return;

            isPaused = switcher;

            if (isPaused)
            {
                savedVelocity = Rb.linearVelocity;
                savedAngularVelocity = Rb.angularVelocity;
                ResetVelocity();
            }
            else
            {
                Rb.linearVelocity = savedVelocity;
                Rb.angularVelocity = savedAngularVelocity;
            }
        }

        public void ResetVelocity()
        {
            Rb.linearVelocity = Vector2.zero;
            Rb.angularVelocity = 0f;
        }

        public void ResetSavedVelocity()
        {
            savedVelocity = Vector2.zero;
            savedAngularVelocity = 0f;
        }

        public void ShowLaserVisual()
        {
            if (_lineRenderer == null) return;
            StartCoroutine(LaserRoutine());
        }
        
        public void ShowBulletVisual()
        {
            _fireBulletParticles.Play();
        }

        public void Revive()
        {
            ResetVelocity();
            ResetSavedVelocity();
            RegisterDependencies();
            gameObject.SetActive(true);
            Teleport(Vector3.zero);
        }

        private IEnumerator LaserRoutine()
        {
            if (_lineRenderer == null) yield break;

            _lineRenderer.enabled = true;

            Vector3 start = transform.position;
            Vector3 end = start + transform.up * LASER_LENGTH;

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);

            yield return _laserVisualLifespan;

            _lineRenderer.enabled = false;
        }

        private void ShootLaser()
        {
            FireLaserRequested?.Invoke();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Dispose()
        {
            if (_playerControls == null) return;

            _playerControls.Player.Disable();

            _playerControls.Player.Move.performed -= context => _moveInput = context.ReadValue<float>();
            _playerControls.Player.Move.canceled -= context => _moveInput = 0f;
            _playerControls.Player.Rotate.performed -= context => _rotateInput = context.ReadValue<float>();
            _playerControls.Player.Rotate.canceled -= context => _rotateInput = 0f;

            _playerControls.Player.ShootBullet.performed -= context => _shootBulletInput = 1f;
            _playerControls.Player.ShootBullet.canceled -= context => _shootBulletInput = 0f;

            _playerControls.Player.ShootLaser.started -= context => ShootLaser();

            _playerControls.Player.Pause.started -= context => OnPauseClick?.Invoke();
        }
        
    }
}