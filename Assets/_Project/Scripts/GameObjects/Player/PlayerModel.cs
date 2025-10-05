using System;
using UnityEngine;

namespace Asteroids
{
    public class PlayerModel
    {
        private const int MAX_LASER_SHOTS = 3;
        private const int SIZE_OF_RAYCASTHITS_ARRAY = 10;
        private const float LASER_DISTANCE = 20f;

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

        private Camera _mainCamera;
        private BulletFactory _bulletFactory;
        private UtilsCalculatePositions _utils;
        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[SIZE_OF_RAYCASTHITS_ARRAY];

        private bool _isBulletRecharging;
        private float _rechargeBulletTimer = 0f;

        public PlayerModel(PlayerConfig playerConfig, Camera camera, BulletFactory bulletFactory, UtilsCalculatePositions utils)
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
            _utils = utils;
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
            return _utils.CheckBounds(transform);
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

        private void IncreaseLaserShots()
        {
            if (LaserShots < MAX_LASER_SHOTS)
            {
                LaserShots++;
            }
        }

        private void DecreaseLaserShots()
        {
            if (LaserShots > 0)
            {
                LaserShots--;
            }
        }
    }
}