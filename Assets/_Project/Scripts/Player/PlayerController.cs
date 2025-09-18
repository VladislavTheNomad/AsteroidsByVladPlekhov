using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PlayerController : MonoBehaviour, IInitializable
    {
        private const float SCREEN_RIGHT_BOUND = 1.1f;
        private const float SCREEN_LEFT_BOUND = -0.1f;
        private const float SCREEN_TOP_BOUND = 1.1f;
        private const float SCREEN_BOTTOM_BOUND = -0.1f;

        [SerializeField, Range(10, 100)] private float _rotationSpeed;
        [SerializeField, Range(1, 20)] private float _movementSpeed;

        private Transform _transform;
        private PlayerControls _playerControls;
        private float _moveInput;
        private float _rotateInput;
        private float _shootBulletInput;
        private Vector3 _bottomLeft;
        private Vector3 _topRight;
        private UIManager _uiManager;
        private Camera _mainCamera;

        private GameObject _playerObject;
        private LaserVisual _laserVisual;
        private FireLaser _fireLaser;
        private FireBullet _fireBullet;
        private Rigidbody2D _rb;
        private LineRenderer _lineRenderer;

        [Inject]
        public void Construct(UIManager uiManager, Camera mainCamera, GameObject playerObject)
        {
            _uiManager = uiManager;
            _mainCamera = mainCamera;
            _playerObject = playerObject;
        }

        private void OnDisable()
        {
            _playerControls?.Player.Disable();
        }

        private void Update()
        {
            if (_shootBulletInput > 0f)
            {
                _fireBullet.ShootBullet();
            }
        }

        private void FixedUpdate()
        {
            _rb.AddTorque(_rotateInput * _rotationSpeed * Time.fixedDeltaTime);
            if (_moveInput > 0f)
            {
                _rb.AddForce(_transform.up * _movementSpeed, ForceMode2D.Force);
                _uiManager.UpdateSpeed(_rb.linearVelocity.magnitude);
            }
        }

        private void LateUpdate()
        {
            Vector3 playerPositionInCameraCoordinates = _mainCamera.WorldToViewportPoint(_transform.position);
            _uiManager.UpdateCoordinates(playerPositionInCameraCoordinates, _transform.eulerAngles.z);

            if (
                playerPositionInCameraCoordinates.x < SCREEN_LEFT_BOUND ||
                playerPositionInCameraCoordinates.x > SCREEN_RIGHT_BOUND ||
                playerPositionInCameraCoordinates.y > SCREEN_TOP_BOUND ||
                playerPositionInCameraCoordinates.y < SCREEN_BOTTOM_BOUND
                )
            {
                Vector3 newPosition = _transform.position;

                if (playerPositionInCameraCoordinates.x > SCREEN_RIGHT_BOUND)
                {
                    newPosition.x = _bottomLeft.x;
                }
                else if (playerPositionInCameraCoordinates.x < SCREEN_LEFT_BOUND)
                {
                    newPosition.x = _topRight.x;
                }

                if (playerPositionInCameraCoordinates.y > SCREEN_TOP_BOUND)
                {
                    newPosition.y = _bottomLeft.y;
                }
                else if (playerPositionInCameraCoordinates.y < SCREEN_BOTTOM_BOUND)
                {
                    newPosition.y = _topRight.y;
                }
                _rb.MovePosition(newPosition);
            }
        }

        public void Initialize()
        {
            _laserVisual = _playerObject.GetComponent<LaserVisual>();
            _fireLaser = _playerObject.GetComponent<FireLaser>();
            _fireBullet = _playerObject.GetComponent<FireBullet>();
            _rb = _playerObject.GetComponent<Rigidbody2D>();
            _lineRenderer = _playerObject.GetComponent<LineRenderer>();
            _transform = _playerObject.transform;

            _laserVisual.SetLineRenderer();
            _fireLaser.SetStartAmountOfShots();
            _fireLaser.SetAmountOfRechargeTimers();

            _bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _transform.position.z));
            _topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _transform.position.z));

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

        private void ShootLaser()
        {
            _fireLaser.Fire();
        }
    }
}