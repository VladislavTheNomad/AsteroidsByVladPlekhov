using UnityEngine;

namespace Asteroids
{
    public class AsteroidSplitting : MonoBehaviour
    {
        private const int NUMBER_OF_SMALL_ASTEROIDS = 2;
        private const float ACCELERATION_MODIFICATOR = 1f;

        [SerializeField] private AsteroidBehaviour _asteroidBehaviour;

        private PoolManager<AsteroidBehaviour> _poolManager;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletBehaviour>()) return;
            if (_asteroidBehaviour._asteroidCurrentSizeLevel > 1)
            {
                for (int i = 0; i < NUMBER_OF_SMALL_ASTEROIDS; i++)
                {
                    AsteroidBehaviour smallAsteroid = _poolManager.Get();
                    smallAsteroid.SetCurrentSizeLevel(_asteroidBehaviour._asteroidCurrentSizeLevel - 1);
                    smallAsteroid.AddAcceleration(ACCELERATION_MODIFICATOR);
                    smallAsteroid.transform.localScale = new Vector3(transform.localScale.x * 0.5f, transform.localScale.y * 0.5f, transform.localScale.z);
                    smallAsteroid.transform.position = transform.position;
                    smallAsteroid.gameObject.SetActive(true);
                }
                _asteroidBehaviour.DeathConditions();
            }
            else
            {
                _asteroidBehaviour.DeathConditions();
            }
        }

        public void Initialize(PoolManager<AsteroidBehaviour> manager)
        {
            _poolManager = manager;
        }
    }
}