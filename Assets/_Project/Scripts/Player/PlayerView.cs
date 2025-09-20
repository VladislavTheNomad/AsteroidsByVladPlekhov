using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PlayerView : MonoBehaviour, IInitializable, IPlayerView
    {
        private const float LASER_WIDTH = 0.2f;
        private const float LASER_DURATION = 0.25f;
        private const float LASER_LENGTH = 20f;

        public event Action MoveRequested;
        public event Action<float> RotateRequested;
        public event Action FireBulletRequested;
        public event Action FireLaserRequested;
        public event Action CollisionDetected;

        private LineRenderer _lineRenderer;
        private PlayerControls _playerControls;

        [field: SerializeField] public GameObject LeftBound { get; private set; }
        [field: SerializeField] public GameObject RightBound { get; private set; }
        public float _moveInput { get; private set; }
        public float _rotateInput { get; private set; }
        public Rigidbody2D Rb { get; private set; }

        private float _shootBulletInput;

        private void OnDisable()
        {
            _playerControls?.Player.Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<AsteroidView>() || collision.GetComponent<UfoView>())
            {
                CollisionDetected?.Invoke();
            }
        }

        public void Update()
        {
            if (_shootBulletInput > 0f)
            {
                FireBulletRequested?.Invoke();
            }
        }

        private void FixedUpdate()
        {
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

            _playerControls = new PlayerControls();
            _playerControls.Player.Enable();
            _playerControls.Player.Move.performed += context => _moveInput = context.ReadValue<float>();
            _playerControls.Player.Move.canceled += context => _moveInput = 0f;
            _playerControls.Player.Rotate.performed += context => _rotateInput = context.ReadValue<float>();
            _playerControls.Player.Rotate.canceled += context => _rotateInput = 0f;

            _playerControls.Player.ShootBullet.performed += context => _shootBulletInput = 1f;
            _playerControls.Player.ShootBullet.canceled += context => _shootBulletInput = 0f;

            _playerControls.Player.ShootLaser.started += context => ShootLaser();
        }

        public void ShowLaserVisual()
        {
            if (_lineRenderer == null) return;
            StartCoroutine(LaserRoutine());
        }

        public void SetLineRenderer()
        {
            _lineRenderer.enabled = false;
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = LASER_WIDTH;
            _lineRenderer.endWidth = LASER_WIDTH;
        }

        private IEnumerator LaserRoutine()
        {
            if (_lineRenderer == null) yield break;

            _lineRenderer.enabled = true;

            Vector3 start = transform.position;
            Vector3 end = start + transform.up * LASER_LENGTH;

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);

            yield return new WaitForSeconds(LASER_DURATION);

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
    }
}