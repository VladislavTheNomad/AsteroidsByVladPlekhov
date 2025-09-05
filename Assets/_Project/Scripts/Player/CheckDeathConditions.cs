using System;
using UnityEngine;

namespace Asteroids
{
    public class CheckDeathConditions : MonoBehaviour
    {
        public event Action OnPlayerIsDead;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<AsteroidBehaviour>() || collision.GetComponent<UfoBehaviour>())
            {
                OnPlayerIsDead?.Invoke();
            }
        }
    }
}