using System;
using UnityEngine;

namespace Asteroids
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Transform))]
    public class AsteroidView : MonoBehaviour, IHaveDeathConditions
    {
        public event Action OnDeath;
        public event Action OnMovement;
        public event Action OnEnabled;
        public event Action OnSetNew;
        public event Action<int, Transform> OnGetSmaller;

        public Transform Transform { get; private set; }

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
            if (!other.TryGetComponent<BulletView>(out BulletView bv)) return;
            OnDeath?.Invoke();
        }

        private void Update()
        {
            if(!_isPaused)
            {
                OnMovement?.Invoke();
            }
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
            Transform = GetComponent<Transform>();
        }

        public void SetNewAsteroid()
        {
            OnSetNew?.Invoke();
        }

        public void GetSmaller(int parentSizeLevel, Transform parentTransform)
        {
            OnGetSmaller?.Invoke(parentSizeLevel, parentTransform);
        }

        public void Move(Vector3 destination, float impulse, float acceleration)
        {
            if (!_isPaused)
            {
                _rb.AddForce(destination * impulse * acceleration, ForceMode2D.Force);
            }
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        public void SetNewPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public void SetActive()
        {
            gameObject.SetActive(true);
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
            else if (!_isPaused)
            {
                _rb.linearVelocity = savedLinearVelocity;
                _rb.angularVelocity = savedAngularVelocity;
            }
        }
    }
}