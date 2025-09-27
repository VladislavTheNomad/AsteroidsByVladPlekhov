using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Asteroids
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Transform))]
    public class UfoView : MonoBehaviour, IHaveDeathConditions
    {
        public event Action OnDeath;
        public event Action OnEnabled;

        public Transform ViewTransform { get; private set; }

        private Rigidbody2D _rb;

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletView>()) return;
            OnDeath?.Invoke();
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
            ViewTransform = GetComponent<Transform>();
        }

        public void Move(Vector3 destination, float moveSpeed)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.AddForce(destination * moveSpeed, ForceMode2D.Impulse);
        }

        public void HandleDeath()
        {
            OnDeath?.Invoke();
        }
    }
}