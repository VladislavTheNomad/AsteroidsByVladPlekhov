using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class FireLaser : MonoBehaviour
    {
        private const int SIZE_OF_RAYCASTHITS_ARRAY = 10;
        private const float LASER_DISTANCE = 20f;

        public event Action OnAmountLaserShotChange;
        public event Action OnRechargeTimer;

        [SerializeField, Range(0, 10)] private int _maxLaserShoots;
        [SerializeField, Range(0.1f, 10f)] private float _rechargeTime;
        [SerializeField] private LayerMask _destructableLayers;
        [SerializeField] private GameObject _leftBound;
        [SerializeField] private GameObject _rightBound;
        [SerializeField] private LaserVisual _laserVisual;

        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[SIZE_OF_RAYCASTHITS_ARRAY];

        public int CurrentLaserShoots { get; private set; }
        public float[] CurrentRechargeTimers { get; private set; }

        private void Update()
        {
            for (int i = 0; i < CurrentRechargeTimers.Length; i++)
            {
                if (CurrentRechargeTimers[i] > 0f)
                {
                    OnRechargeTimer?.Invoke();
                    break;
                }
            }
        }

        public void Fire()
        {
            if (CurrentLaserShoots > 0)
            {
                _laserVisual.ShowLaserVisual();
                int slotIndex = CurrentLaserShoots - 1;
                CurrentLaserShoots--;
                OnAmountLaserShotChange?.Invoke();
                StartCoroutine(Recharge(slotIndex));
                RayCastGo(_leftBound);
                RayCastGo(_rightBound);
            }
        }

        public IEnumerator Recharge(int index)
        {
            CurrentRechargeTimers[index] = _rechargeTime;
            while (CurrentRechargeTimers[index] > 0f)
            {
                CurrentRechargeTimers[index] -= Time.deltaTime;
                yield return null;
            }
            CurrentRechargeTimers[index] = 0f;
            CurrentLaserShoots++;
            OnAmountLaserShotChange?.Invoke();
        }

        public void SetStartAmountOfShots() => CurrentLaserShoots = _maxLaserShoots;

        public void SetAmountOfRechargeTimers() => CurrentRechargeTimers = new float[_maxLaserShoots];

        public int GetMaxShots() => _maxLaserShoots;

        private void RayCastGo(GameObject bound)
        {
            int hits = Physics2D.RaycastNonAlloc(bound.transform.position, transform.up, _raycastHits, LASER_DISTANCE, _destructableLayers);

            for (int i = 0; i < hits; i++)
            {
                if (_raycastHits[i].collider.TryGetComponent<IHaveDeathConditions>(out var objectToDestroy))
                {
                    objectToDestroy.DeathConditions();
                }
            }
        }
    }
}