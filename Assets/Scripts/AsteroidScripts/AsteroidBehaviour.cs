using UnityEngine;

namespace Asteroids
{
    public class AsteroidBehaviour : MonoBehaviour
    {
        //own
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
            Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(Vector2.zero);
            Vector2 topRight = mainCamera.ViewportToWorldPoint(Vector2.one);

            Vector3 targetPoint = new Vector3(Random.Range(bottomLeft.x, topRight.x), Random.Range(bottomLeft.y, topRight.y), 0);
            destination = (targetPoint - transform.position).normalized;

            moveSpeed = isBigOne ? Random.Range(minMoveSpeed, maxMoveSpeed) : Random.Range(minMoveSpeed * acceleration, maxMoveSpeed * acceleration);
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
