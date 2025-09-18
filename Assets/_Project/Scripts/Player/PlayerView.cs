using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PlayerView : MonoBehaviour, IInitializable
    {
        private const float LASER_WIDTH = 0.2f;
        private const float LASER_DURATION = 0.25f;
        private const float LASER_LENGTH = 20f;

        private LineRenderer _lineRenderer;
        private PlayerPresenter _presenter;
        private PlayerModel _model;
        private UIManager _uiManager;
        private GamePoolsController _gamePoolsController;

        private PlayerControls _playerControls;
        public float _moveInput { get; private set; }
        public float _rotateInput { get; private set; }

        private float _shootBulletInput;

        [Inject]
        public void Construct(PlayerPresenter pp, PlayerModel pm, UIManager uiManager, GamePoolsController gamePoolsController)
        {
            _presenter = pp;
            _model = pm;
            _uiManager = uiManager;
            _gamePoolsController = gamePoolsController;
        }

        private void OnDisable()
        {
            _playerControls?.Player.Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<AsteroidBehaviour>() || collision.GetComponent<UfoBehaviour>())
            {
                _presenter.PlayerIsDead();
            }
        }

        public void Update()
        {
            if (_shootBulletInput > 0f)
            {
                _presenter.FireBullet();
            }
            UpdateUI();
        }

        private void FixedUpdate()
        {
            _presenter.AddTorque(_rotateInput);

            if (_moveInput > 0f)
            {
                _presenter.AddMove();
            }
        }

        public void Initialize()
        {
            _lineRenderer = GetComponent<LineRenderer>();

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

        public void UpdateUI()
        {
            _uiManager.UpdateSpeed(_model.Speed);
            _uiManager.UpdateCoordinates(_model.Position, _model.Rotation);
        }

        public void SpawnBullet()
        {
            BulletPresenter bulletSpawn = _gamePoolsController.GetBulletPool().Get();
            bulletSpawn.transform.SetPositionAndRotation(transform.position, transform.rotation);
            bulletSpawn.gameObject.SetActive(true);
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
            _presenter.FireLazer();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}