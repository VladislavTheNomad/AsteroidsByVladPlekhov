using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class UfoView : MonoBehaviour
    {
        public event Action OnDeath;

        private Rigidbody2D _rb;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletView>()) return;
            OnDeath?.Invoke();
        }

        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void DoMove(Vector3 destination, float moveSpeed)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.AddForce(destination * moveSpeed, ForceMode2D.Impulse);
        }
    }
}
