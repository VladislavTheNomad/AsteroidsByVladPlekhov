using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class BulletBehaviour : MonoBehaviour, IHaveDeathConditions
    {
        public event Action<BulletBehaviour> OnDeath;

        [SerializeField] private float _bulletsLifeTime;
        [SerializeField] private float _moveSpeed;

        private Rigidbody2D _rb;
        private bool _isInitialized;

        private void OnEnable()
        {
            if (_isInitialized)
            {
                StartCoroutine(LifeTime());
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<AsteroidBehaviour>(out var asteroid) || collision.TryGetComponent<UfoBehaviour>(out var ufo))
            {
                DeathConditions();
            }
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
            _isInitialized = true;
        }

        public void DeathConditions()
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            OnDeath?.Invoke(this);
        }

        private IEnumerator LifeTime()
        {
            _rb.AddForce(transform.up * _moveSpeed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(_bulletsLifeTime);
            DeathConditions();
        }
    }
}