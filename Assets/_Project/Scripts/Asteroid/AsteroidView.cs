using System;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Asteroids
{
    public class AsteroidView : MonoBehaviour
    {
        public event Action OnDeath;
        public event Action OnMovement;

        private Rigidbody2D _rb;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletView>()) return;
            OnDeath?.Invoke();
        }

        private void Update()
        {
            OnMovement?.Invoke();
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void DoMove(Vector3 destination, float impulse, float acceleration)
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
    }
}