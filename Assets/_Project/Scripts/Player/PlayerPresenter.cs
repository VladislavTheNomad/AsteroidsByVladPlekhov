using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Asteroids
{
    public class PlayerPresenter : MonoBehaviour, IInitializable
    {
        private const float SCREEN_RIGHT_BOUND = 1.1f;
        private const float SCREEN_LEFT_BOUND = -0.1f;
        private const float SCREEN_TOP_BOUND = 1.1f;
        private const float SCREEN_BOTTOM_BOUND = -0.1f;
        private const int SIZE_OF_RAYCASTHITS_ARRAY = 10;
        private const float LASER_DISTANCE = 20f;

        public event Action OnRechargeTimer;
        public event Action OnAmountLaserShotChange;
        public event Action OnPlayerIsDead;

        [SerializeField, Range(10, 100)] private float _rotationSpeed;
        [SerializeField, Range(1, 20)] private float _movementSpeed;
        [SerializeField, Range(0, 1)] private float _bulletRechargeTime = 0.3f;
        [SerializeField, Range(1, 30)] private float _laserRechargeTime = 10f;
        [SerializeField] private GameObject _leftBound;
        [SerializeField] private GameObject _rightBound;
        [SerializeField] private LayerMask _destructableLayers;

        private Rigidbody2D _rbPlayer;
        private Camera _mainCamera;
        private PlayerModel _model;
        private PlayerView _view;
        private Vector3 _bottomLeft;
        private Vector3 _topRight;
        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[SIZE_OF_RAYCASTHITS_ARRAY];

        [Inject]
        public void Construct(PlayerModel playermodel, PlayerView playerView, Camera mainCamera, Rigidbody2D rb)
        {
            _model = playermodel;
            _mainCamera = mainCamera;
            _view = playerView;
            _rbPlayer = rb;
        }

        private void LateUpdate()
        {
            Vector3 playerPositionInCameraCoordinates = _mainCamera.WorldToViewportPoint(_view.transform.position);
            _model.UpdatePosition(playerPositionInCameraCoordinates);
            _model.UpdateRotation(_view.transform.eulerAngles.z);

            if (
                playerPositionInCameraCoordinates.x < SCREEN_LEFT_BOUND ||
                playerPositionInCameraCoordinates.x > SCREEN_RIGHT_BOUND ||
                playerPositionInCameraCoordinates.y > SCREEN_TOP_BOUND ||
                playerPositionInCameraCoordinates.y < SCREEN_BOTTOM_BOUND
                )
            {
                Vector3 newPosition = _view.transform.position;

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
                _rbPlayer.MovePosition(newPosition);
            }
        }

        public void Initialize()
        {
            _bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _view.transform.position.z));
            _topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _view.transform.position.z));

            _model.SetBulletRechargeTime(_bulletRechargeTime);
            _model.SetLaserRechargeTime(_laserRechargeTime);
        }

        public void AddTorque(float input)
        {
            _rbPlayer.AddTorque(input * _rotationSpeed * Time.deltaTime);
        }

        public void AddMove()
        {
            _rbPlayer.AddForce(_view.transform.up * _movementSpeed, ForceMode2D.Force);
            _model.UpdateSpeed(_rbPlayer.linearVelocity.magnitude);
        }

        public void PlayerIsDead()
        {
            OnPlayerIsDead?.Invoke();
        }

        public void FireBullet()
        {
            if (_model.CanFireBullet)
            {
                StartCoroutine(RechargeBullet());
                _view.SpawnBullet();
            }
        }

        public void FireLazer()
        {
            if (_model.LaserShots > 0)
            {
                _view.ShowLaserVisual();
                int slotIndex = _model.LaserShots - 1;
                _model.DecreaseLaserShots();
                OnAmountLaserShotChange?.Invoke();
                StartCoroutine(RechargeLazer(slotIndex));
                RayCastGo(_leftBound);
                RayCastGo(_rightBound);
            }
        }

        private IEnumerator RechargeBullet()
        {
            _model.SetCanFireBullet(false);
            yield return new WaitForSeconds(_bulletRechargeTime);
            _model.SetCanFireBullet(true);
        }

        private IEnumerator RechargeLazer(int index)
        {
            _model.LaserRechargeTimers[index] = _model.LaserRechargeTime;

            while (_model.LaserRechargeTimers[index] > 0f)
            {
                _model.SubtractLazerRechargeTimer(index, Time.deltaTime);

                OnRechargeTimer?.Invoke();
                yield return null;
            }
            _model.SetLazerRechargeTimer(index, 0f);
            _model.IncreaseLaserShots();
            OnAmountLaserShotChange?.Invoke();
        }

        private void RayCastGo(GameObject bound)
        {
            int hits = Physics2D.RaycastNonAlloc(bound.transform.position, _view.transform.up, _raycastHits, LASER_DISTANCE, _destructableLayers);

            for (int i = 0; i < hits; i++)
            {
                if (_raycastHits[i].collider.TryGetComponent<IHaveDeathConditions>(out var objectToDestroy))
                {
                    objectToDestroy.DeathConditions();
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}