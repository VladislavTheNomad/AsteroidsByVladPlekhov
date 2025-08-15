using UnityEngine;

namespace Asteroids
{
    public class AsteroidBehaviour : MonoBehaviour
    {
        private AsteroidPoolManager asteroidPoolManager;
        private Camera mainCamera;
        private Vector3 destination;
        private float moveSpeed;
        public bool isBigOne { get; private set; }

        //settings
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float minMoveSpeed;
        [SerializeField] private int acceleration;

        private void OnEnable()
        {
            mainCamera = Camera.main;

            Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
            Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

            Vector3 targetPoint = new Vector3(Random.Range(bottomLeft.x, topRight.x), Random.Range(bottomLeft.y, topRight.y), 0);

            destination = (targetPoint - transform.position).normalized;
            if (!isBigOne)
            {
                moveSpeed = Random.Range(minMoveSpeed * acceleration, maxMoveSpeed * acceleration);
            }
            else { moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed); }
        }


        private void Update()
        {
            transform.position += destination * moveSpeed * Time.deltaTime;

            Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
            if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y > 1.1f || viewportPos.y <-0.1f)
            {
                asteroidPoolManager.ReturnAsteroid(gameObject);
            }
        }
        public void SetPoolManager(AsteroidPoolManager manager)
        {
            asteroidPoolManager = manager;
        }

        public void SetBigForm() => isBigOne = true;
        public void RemoveBigForm() => isBigOne = false;

    }
}
