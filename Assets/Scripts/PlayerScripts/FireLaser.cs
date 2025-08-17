using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class FireLaser : MonoBehaviour
    {
        //settings
        [SerializeField] private int maxLaserShoots;
        [SerializeField] private float rechargeTime;
        [SerializeField] private float laserDistance;
        [SerializeField] private LayerMask destructableLayers;

        //connections
        [SerializeField] GameObject leftBound;
        [SerializeField] GameObject rightBound;
        [SerializeField] private LaserVisual laserVisual;

        //own
        public int currentLaserShoots { get; private set; }
        public float[] currentRechageTimers { get; private set; }

        public event Action OnAmountLaserShotChange;
        public event Action OnRechangeTimer;
        private readonly RaycastHit2D[] raycastHits = new RaycastHit2D[10];

        public void Fire()
        {
            if(currentLaserShoots > 0)
            {
                laserVisual.ShowLaserVisual();
                int slotIndex = currentLaserShoots - 1;
                currentLaserShoots--;
                OnAmountLaserShotChange?.Invoke();
                StartCoroutine(Recharge(slotIndex));
                RayCastGo(leftBound);
                RayCastGo(rightBound);
            }
        }

        private void Update()
        {
            OnRechangeTimer?.Invoke();
        }

        private void RayCastGo(GameObject bound)
        {
            int hits = Physics2D.RaycastNonAlloc(bound.transform.position, transform.up, raycastHits, laserDistance, destructableLayers);

            for (int i = 0; i < hits; i++)
            {
                if(raycastHits[i].collider.TryGetComponent<IHaveDeathConditions>(out var objectToDestroy))
                {
                    objectToDestroy.DeathConditions();
                }
            }
        }

        public IEnumerator Recharge(int index)
        {
            currentRechageTimers[index] = rechargeTime;
            while (currentRechageTimers[index] > 0f)
            {
                currentRechageTimers[index] -= Time.deltaTime;
                yield return null;
            }
            currentRechageTimers[index] = 0f;
            currentLaserShoots++;
            OnAmountLaserShotChange?.Invoke();
        }

        public void SetStartAmountOfShots() => currentLaserShoots = maxLaserShoots;

        public void SetAmountOfRechareTimers() => currentRechageTimers = new float[maxLaserShoots];

        public int GetMaxShots() => maxLaserShoots;
    }
}