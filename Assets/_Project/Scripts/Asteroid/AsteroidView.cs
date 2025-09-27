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
            OnMovement?.Invoke();
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
            _rb.AddForce(destination * impulse * acceleration, ForceMode2D.Force);
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
    }
}