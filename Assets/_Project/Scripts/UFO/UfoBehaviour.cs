using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class UfoBehaviour : MonoBehaviour, IHaveDeathConditions, IPoolable<UfoBehaviour>, IGetPointsOnDestroy
    {
        private const int SCORE_POINTS = 1;

        public event Action<int> OnDeathTakeScore;
        public event Action<UfoBehaviour> OnDeathReturnToPool;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _gapBetweenPositionChanging;

        private Vector3 _destination;
        private GameObject playerObject;
        private Rigidbody2D _rb;
        private bool _isInitialized;
        private Coroutine _moveCoroutine;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (gameObject.activeSelf && _isInitialized && _moveCoroutine == null)
            {
                _moveCoroutine = StartCoroutine(MoveBehaviour());
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletBehaviour>()) return;
            DeathConditions();
        }

        public void Initialize(GameObject player)
        {
            playerObject = player;
            _rb = GetComponent<Rigidbody2D>();
            _isInitialized = true;
        }

        public void DeathConditions()
        {
            OnDeathReturnToPool.Invoke(this);
            OnDeathTakeScore.Invoke(SCORE_POINTS);
        }

        private IEnumerator MoveBehaviour()
        {
            while (true)
            {
                yield return new WaitForSeconds(_gapBetweenPositionChanging);
                _destination = (playerObject.transform.position - transform.position).normalized;
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.AddForce(_destination * _moveSpeed, ForceMode2D.Impulse);
            }
        }
    }
}