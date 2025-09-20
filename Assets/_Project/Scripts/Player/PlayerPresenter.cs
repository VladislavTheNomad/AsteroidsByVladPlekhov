using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Asteroids
{
    public class PlayerPresenter : MonoBehaviour, IInitializable, IHaveDeathConditions
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

        private Camera _mainCamera;
        private PlayerModel _model;
        private PlayerView _view;
        private UIManager _uiManager;
        private Vector3 _bottomLeft;
        private Vector3 _topRight;
        private GamePoolsController _gamePoolsController;
        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[SIZE_OF_RAYCASTHITS_ARRAY];

        [Inject]
        public void Construct(PlayerModel playermodel, UIManager uiManager, PlayerView playerView, Camera mainCamera, GamePoolsController gamePoolsController)
        {
            _model = playermodel;
            _mainCamera = mainCamera;
            _view = playerView;
            _gamePoolsController = gamePoolsController;
            _uiManager = uiManager;
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
                _view.Rb.MovePosition(newPosition);
            }
            UpdateUI();
        }

        public void Initialize()
        {
            _bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _view.transform.position.z));
            _topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _view.transform.position.z));

            _view.MoveRequested += AddMove;
            _view.RotateRequested += AddTorque;
            _view.FireBulletRequested += FireBullet;
            _view.FireLaserRequested += FireLaser;
            _view.CollisionDetected += DeathConditions;
        }

        public void AddTorque(float input)
        {
            _view.Rb.AddTorque(input * _model.RotationSpeed * Time.deltaTime);
        }

        public void AddMove()
        {
            _view.Rb.AddForce(_view.transform.up * _model.MovementSpeed, ForceMode2D.Force);
            _model.UpdateSpeed(_view.Rb.linearVelocity.magnitude);
        }

        public void FireBullet()
        {
            if (_model.CanFireBullet)
            {
                StartCoroutine(RechargeBullet());
                SpawnBullet();
            }
        }

        public void SpawnBullet()
        {
            BulletPresenter bulletSpawn = _gamePoolsController.GetBulletPool().Get();
            bulletSpawn.transform.SetPositionAndRotation(_view.transform.position, _view.transform.rotation);
            bulletSpawn.gameObject.SetActive(true);
        }

        public void FireLaser()
        {
            if (_model.LaserShots > 0)
            {
                _view.ShowLaserVisual();
                int slotIndex = _model.LaserShots - 1;
                _model.DecreaseLaserShots();
                OnAmountLaserShotChange?.Invoke();
                StartCoroutine(RechargeLazer(slotIndex));
                RayCastGo(_view.LeftBound);
                RayCastGo(_view.RightBound);
            }
        }

        public void DeathConditions()
        {
            OnPlayerIsDead?.Invoke();
        }

        public void UpdateUI()
        {
            _uiManager.UpdateSpeed(_model.CurrentSpeed);
            _uiManager.UpdateCoordinates(_model.Position, _model.Rotation);
        }

        private IEnumerator RechargeBullet()
        {
            _model.SetCanFireBullet(false);
            yield return new WaitForSeconds(_model.BulletRechargeTime);
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
            int hits = Physics2D.RaycastNonAlloc(bound.transform.position, _view.transform.up, _raycastHits, LASER_DISTANCE, _model.DestructableLayers);

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

            _view.MoveRequested -= AddMove;
            _view.RotateRequested -= AddTorque;
            _view.FireBulletRequested -= FireBullet;
            _view.FireLaserRequested -= FireLaser;
            _view.CollisionDetected -= DeathConditions;
        }
    }
}