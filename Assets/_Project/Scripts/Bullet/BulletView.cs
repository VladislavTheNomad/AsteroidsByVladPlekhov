using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class BulletView : MonoBehaviour, IInitializable
    {
        public event Action OnEnabled;
        public event Action OnDisabled;
        public event Action OnHit;

        private Rigidbody2D _rb;

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }

        private void OnDisable()
        {
            OnDisabled?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<AsteroidBehaviour>(out var asteroid) || collision.TryGetComponent<UfoBehaviour>(out var ufo))
            {
                OnHit?.Invoke();
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
    }
}