using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletView : MonoBehaviour
    {
        public event Action OnEnabled;
        public event Action OnDeath;
        public event Action<float> OnLifeTimeSpendingFreeze;

        private Rigidbody2D _rb;
        private bool _isPaused;
        private Vector2 savedLinearVelocity;
        private float savedAngularVelocity;

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<AsteroidView>(out var asteroid) || collision.TryGetComponent<UfoView>(out var ufo))
            {
                OnDeath?.Invoke();
            }
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void MoveBullet(float moveSpeed)
        {
            _rb.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);
        }

        public void StopBulletMovement()
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        public IEnumerator LifeSpan(float currentLifeTime, float maxLifeTime)
        {
            while (currentLifeTime < maxLifeTime && !_isPaused)
            {
                currentLifeTime += Time.deltaTime;
                yield return null;
            }

            if (_isPaused)
            {
                OnLifeTimeSpendingFreeze?.Invoke(currentLifeTime);
            }
            else
            {
                OnDeath?.Invoke();
            }
        }

        public void TogglePause(bool switcher)
        {
            _isPaused = switcher;

            if(_isPaused)
            {
                savedLinearVelocity = _rb.linearVelocity;
                savedAngularVelocity = _rb.angularVelocity;
                StopBulletMovement();
            }
            else
            {
                _rb.linearVelocity = savedLinearVelocity;
                _rb.angularVelocity = savedAngularVelocity;
            }
        }
    }
}