using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PlayerModel : ITickable, IDisposable
    {
        private const int SIZE_OF_RAYCASTHITS_ARRAY = 10;

        public event Action<bool> IsGamePaused;
        public event Action BulletFired;
        public event Action LaserFired;
        public event Action ReviveRequest;
        public Vector3 Position { get; private set; }
        public float Rotation { get; private set; }
        public float CurrentSpeed { get; private set; }
        public float MovementSpeed { get; private set; }
        public float RotationSpeed { get; private set; }
        public int MaxLaserShots { get; private set; }
        public int CurrentLaserShots { get; private set; }
        public float LaserDistance { get; private set; }
        public float BulletRechargeTime { get; private set; }
        public float LaserRechargeTime { get; private set; }
        public float[] LaserRechargeTimers { get; private set; }
        public LayerMask DestructableLayers { get; private set; }
        public bool IsInvincible { get; private set; } = false;
        public float InvincibleTime { get; private set; }

        private BulletFactory _bulletFactory;
        private HUDModel _HUDModel;
        private PauseGame _pauseManager;
        private UtilsCalculatePositions _utils;
        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[SIZE_OF_RAYCASTHITS_ARRAY];


        private bool _isBulletRecharging;
        private float _rechargeBulletTimer = 0f;

        [Inject]
        public PlayerModel(BulletFactory bf, UtilsCalculatePositions utils, HUDModel hudModel, PauseGame pm, RemoteConfigService rcs)
        {
            Position = Vector3.zero;
            Rotation = 0f;
            CurrentSpeed = 0f;

            MovementSpeed = rcs.Config.MovementSpeed;
            RotationSpeed = rcs.Config.RotationSpeed;
            BulletRechargeTime = rcs.Config.BulletRechargeTime;
            LaserRechargeTime = rcs.Config.LaserRechargeTime;
            DestructableLayers = rcs.Config.DestructableLayers;
            MaxLaserShots = rcs.Config.MaxLaserShots;
            LaserDistance = rcs.Config.LaserDistance;
            InvincibleTime = rcs.Config.InvincibleTime;

            CurrentLaserShots = MaxLaserShots;
            LaserRechargeTimers = new float[MaxLaserShots];

            for (int i = 0; i < LaserRechargeTimers.Length; i++)
            {
                LaserRechargeTimers[i] = 0f;
            }

            _bulletFactory = bf;
            _utils = utils;
            _HUDModel = hudModel;
            _pauseManager = pm;

            _pauseManager.GameIsPaused += TogglePause;
            _HUDModel.OnRevive += RevivePlayer;

            _HUDModel.SetMaxLaserShots(GetMaxLaserShots());
        }

        public void Dispose()
        {
            _pauseManager.GameIsPaused -= TogglePause;
            _HUDModel.OnRevive -= RevivePlayer;
        }

        public void Tick()
        {
            CheckBulletRecharge(Time.deltaTime);
            CheckLaserRecharge(Time.deltaTime);
            UpdateUI();
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
            if (CurrentLaserShots >= MaxLaserShots) return;
            
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
                BulletFired?.Invoke();
                _isBulletRecharging = true;
            }
        }

        public void SpawnBullet(Transform transform)
        {
            BulletView bulletSpawn = _bulletFactory.GetBulletFromPool(transform);
        }

        public void FireLaser(out bool canFire)
        {
            if (CurrentLaserShots > 0)
            {
                LaserFired?.Invoke();
                int rechargeSlotIndex = -1;
                for (int i = 0; i < MaxLaserShots; i++)
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

        public void RequestDeathConditions()
        {
            _HUDModel.PlayerDead();
        }

        public void RayCastGo(GameObject bound, Transform transform)
        {
            int hits = Physics2D.RaycastNonAlloc(bound.transform.position, transform.up, _raycastHits, LaserDistance, DestructableLayers);

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

        public void PauseGame() => _pauseManager.PauseGameProcess();

        public int GetMaxLaserShots() => MaxLaserShots;

        public void SetPosition(Vector3 pos) => Position = pos;
        public void SetRotation(float rotation) => Rotation = rotation;

        private async void RevivePlayer()
        {
            IsInvincible = true;
            ReviveRequest?.Invoke();
            await Task.Delay((int)(1000 * InvincibleTime));
            IsInvincible = false;
        }

        private void TogglePause(bool condition)
        {
            switch (condition)
            {
                case true:
                    IsGamePaused?.Invoke(true);
                    break;
                case false:
                    IsGamePaused?.Invoke(false);
                    break;
            }
        }

        private void IncreaseLaserShots()
        {
            if (CurrentLaserShots < MaxLaserShots)
            {
                CurrentLaserShots++;
            }
        }

        private void DecreaseLaserShots()
        {
            if (CurrentLaserShots > 0)
            {
                CurrentLaserShots--;
            }
        }

        private void UpdateUI()
        {
            _HUDModel.UpdateRechargeTimer(LaserRechargeTimers.Min());
            _HUDModel.UpdateCurrentShots(CurrentLaserShots);
            _HUDModel.UpdateSpeed(CurrentSpeed);
            _HUDModel.UpdateCoordinates(Position, Rotation);
        }        
    }
}