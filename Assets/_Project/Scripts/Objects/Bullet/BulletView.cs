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

        private Rigidbody2D _rigidBody;
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
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void MoveBullet(float moveSpeed)
        {
            _rigidBody.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);
        }

        public void StopBulletMovement()
        {
            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
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
                savedLinearVelocity = _rigidBody.linearVelocity;
                savedAngularVelocity = _rigidBody.angularVelocity;
                StopBulletMovement();
            }
            else
            {
                _rigidBody.linearVelocity = savedLinearVelocity;
                _rigidBody.angularVelocity = savedAngularVelocity;
            }
        }
    }
}