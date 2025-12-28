using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Asteroids
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Transform))]
    public class UfoView : MonoBehaviour, IHaveDeathConditions
    {
        [SerializeField] private ParticleSystem _dyingParticles;
        
        public event Action OnDeath;
        public event Action OnEnabled;

        public Transform ViewTransform { get; private set; }

        private Rigidbody2D _rigidBody;
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
            HandleDeath();
        }

        public void Initialize()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            ViewTransform = GetComponent<Transform>();
        }

        public void Move(Vector3 destination, float moveSpeed)
        {
            if (_isPaused) return;

            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            _rigidBody.AddForce(destination * moveSpeed, ForceMode2D.Impulse);
        }

        public void HandleDeath()
        {
            _dyingParticles.transform.parent = null;
            _dyingParticles.Play();
            OnDeath?.Invoke();
        }

        public void TogglePause(bool switcher)
        {
            _isPaused = switcher;

            if (_isPaused)
            {
                savedLinearVelocity = _rigidBody.linearVelocity;
                savedAngularVelocity = _rigidBody.angularVelocity;
                _rigidBody.linearVelocity = Vector2.zero;
                _rigidBody.angularVelocity = 0f;
            }
            else if(!_isPaused)
            {
                _rigidBody.linearVelocity = savedLinearVelocity;
                _rigidBody.angularVelocity = savedAngularVelocity;
            }
        }
    }
}