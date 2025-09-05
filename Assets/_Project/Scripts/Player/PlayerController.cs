using UnityEngine;

namespace Asteroids
{
    public class PlayerController : MonoBehaviour, IInitiable
    {
        private const float SCREEN_RIGHT_BOUND = 1f;
        private const float SCREEN_LEFT_BOUND = 0f;
        private const float SCREEN_TOP_BOUND = 1f;
        private const float SCREEN_BOTTOM_BOUND = 0f;

        [SerializeField] private GameObject _playerObject;
        [SerializeField] private FireBullet _fireBulletScript;
        [SerializeField] private FireLaser _fireLaserScript;
        [SerializeField] private LaserVisual _laserVisual;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField, Range(10, 100)] private float _rotationSpeed;
        [SerializeField, Range(1, 20)] private float _movementSpeed;

        private Transform _transform;
        private PlayerControls _playerControls;
        private float _moveInput;
        private float _rotateInput;
        private float _shootBulletInput;
        private Vector3 _bottomLeft;
        private Vector3 _topRight;

        private void OnDisable()
        {
            _playerControls.Player.Disable();
        }

        private void Update()
        {
            if (_shootBulletInput > 0f)
            {
                _fireBulletScript.ShootBullet();
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

        public void Installation()
        {
            _laserVisual.SetLineRenderer(_lineRenderer);

            _fireLaserScript.SetStartAmountOfShots();
            _fireLaserScript.SetAmountOfRechargeTimers();

            _transform = _playerObject.transform;

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
            _fireLaserScript.Fire();
        }
    }
}