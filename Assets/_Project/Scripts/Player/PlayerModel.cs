using System;
using UnityEngine;

namespace Asteroids
{
    public class PlayerModel
    {
        private const int MAX_LASER_SHOTS = 3;
        private const float SCREEN_RIGHT_BOUND = 1.1f;
        private const float SCREEN_LEFT_BOUND = -0.1f;
        private const float SCREEN_TOP_BOUND = 1.1f;
        private const float SCREEN_BOTTOM_BOUND = -0.1f;
        private const int SIZE_OF_RAYCASTHITS_ARRAY = 10;
        private const float LASER_DISTANCE = 20f;

        public event Action OnAmountLaserShotChange;

        public Vector3 Position { get; private set; }
        public float Rotation { get; private set; }
        public float CurrentSpeed { get; private set; }
        public float MovementSpeed { get; private set; }
        public float RotationSpeed { get; private set; }
        public int LaserShots { get; private set; }
        public float BulletRechargeTime { get; private set; }
        public float LaserRechargeTime { get; private set; }
        public float[] LaserRechargeTimers { get; private set; }
        public LayerMask DestructableLayers { get; private set; }

        private Vector3 _bottomLeft;
        private Vector3 _topRight;
        private Camera _mainCamera;
        private BulletFactory _bulletFactory;
        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[SIZE_OF_RAYCASTHITS_ARRAY];

        private bool _isBulletRecharging;
        private float _rechargeBulletTimer = 0f;

        public PlayerModel(PlayerConfig playerConfig, Camera camera, BulletFactory bulletFactory)
        {
            Position = Vector3.zero;
            Rotation = 0f;
            CurrentSpeed = 0f;
            LaserShots = MAX_LASER_SHOTS;
            LaserRechargeTimers = new float[MAX_LASER_SHOTS];
            for (int i = 0; i < LaserRechargeTimers.Length; i++)
            {
                LaserRechargeTimers[i] = 0f;
            }

            MovementSpeed = playerConfig.MovementSpeed;
            RotationSpeed = playerConfig.RotationSpeed;
            BulletRechargeTime = playerConfig.BulletRechargeTime;
            LaserRechargeTime = playerConfig.LaserRechargeTime;
            DestructableLayers = playerConfig.DestructableLayers;
            _mainCamera = camera;
            _bulletFactory = bulletFactory;

            _bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
            _topRight = _mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        }

        public void CheckBulletRecharge(float deltaTime)
        {
            if (_isBulletRecharging)
            {
                _rechargeBulletTimer += deltaTime;
                if (_rechargeBulletTimer >= BulletRechargeTime)
                {
                    _rechargeBulletTimer = 0f;
                    _isBulletRecharging = false;
                }
            }
        }

        public void CheckLaserRecharge(float deltaTime)
        {
            if (LaserShots >= MAX_LASER_SHOTS) return;
            
            for (int i = 0; i < LaserRechargeTimers.Length; i++)
            {
                if (LaserRechargeTimers[i] > 0f)
                {
                    LaserRechargeTimers[i] -= deltaTime;

                    if(LaserRechargeTimers[i] <= 0f)
                    {
                        LaserRechargeTimers[i] = 0f;
                        IncreaseLaserShots();
                    }
                }
            }
        }

        public Vector3 CheckBounds(Transform transform)
        {
            Vector3 playerPositionInCameraCoordinates = _mainCamera.WorldToViewportPoint(transform.position);
            UpdatePosition(playerPositionInCameraCoordinates);
            UpdateRotation(transform.eulerAngles.z);

            if (
                playerPositionInCameraCoordinates.x < SCREEN_LEFT_BOUND ||
                playerPositionInCameraCoordinates.x > SCREEN_RIGHT_BOUND ||
                playerPositionInCameraCoordinates.y > SCREEN_TOP_BOUND ||
                playerPositionInCameraCoordinates.y < SCREEN_BOTTOM_BOUND
                )
            {
                Vector3 newPosition = transform.position;

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
                return newPosition;
            }
            return Vector3.zero;
        }

        public void FireBullet(Transform transform)
        {
            if (!_isBulletRecharging)
            {
                SpawnBullet(transform);
                _isBulletRecharging = true;
            }
        }

        public void SpawnBullet(Transform transform)
        {
            BulletView bulletSpawn = _bulletFactory.GetBulletFromPool(transform);
        }

        public void FireLaser(out bool canFire)
        {
            if (LaserShots > 0)
            {
                int rechargeSlotIndex = -1;
                for (int i = 0; i < MAX_LASER_SHOTS; i++)
                {
                    if (LaserRechargeTimers[i] <= 0f)
                    {
                        rechargeSlotIndex = i;
                        break;
                    }
                }

                if (rechargeSlotIndex != -1)
                {
                    LaserRechargeTimers[rechargeSlotIndex] = LaserRechargeTime;
                    DecreaseLaserShots();
                    canFire = true;
                    return;
                }
            }

            canFire = false;
        }

        public void RayCastGo(GameObject bound, Transform transform)
        {
            int hits = Physics2D.RaycastNonAlloc(bound.transform.position, transform.up, _raycastHits, LASER_DISTANCE, DestructableLayers);

            for (int i = 0; i < hits; i++)
            {
                if (_raycastHits[i].collider.TryGetComponent<IHaveDeathConditions>(out var objectToDestroy))
                {
                    objectToDestroy.HandleDeath();
                }
            }
        }

        public void UpdateSpeed(float newSpeed)
        {
            CurrentSpeed = newSpeed;
        }

        public int GetMaxLaserShots() => MAX_LASER_SHOTS;

        private void UpdatePosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        private void UpdateRotation(float newRotation)
        {
            Rotation = newRotation;
        }

        private void IncreaseLaserShots()
        {
            if (LaserShots < MAX_LASER_SHOTS)
            {
                LaserShots++;
                OnAmountLaserShotChange?.Invoke();
            }
        }

        private void DecreaseLaserShots()
        {
            if (LaserShots > 0)
            {
                LaserShots--;
                OnAmountLaserShotChange?.Invoke();
            }
        }
    }
}