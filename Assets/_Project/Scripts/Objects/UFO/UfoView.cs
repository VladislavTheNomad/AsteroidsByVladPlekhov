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
        private bool _isPaused;
        private Vector2 savedLinearVelocity;
        private float savedAngularVelocity;

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
            if (_isPaused) return;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.AddForce(destination * moveSpeed, ForceMode2D.Impulse);
        }

        public void HandleDeath()
        {
            OnDeath?.Invoke();
        }

        public void TogglePause(bool switcher)
        {
            _isPaused = switcher;

            if (_isPaused)
            {
                savedLinearVelocity = _rb.linearVelocity;
                savedAngularVelocity = _rb.angularVelocity;
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
            }
            else if(!_isPaused)
            {
                _rb.linearVelocity = savedLinearVelocity;
                _rb.angularVelocity = savedAngularVelocity;
            }
        }
    }
}