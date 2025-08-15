using UnityEngine;

namespace Asteroids
{
    public class AsteroidSplitting : MonoBehaviour
    {
        private AsteroidPoolManager asteroidPoolManager;

        //connections
        [SerializeField] private AsteroidBehaviour asteroidBehaviour;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletBehaviour>()) return;
            if (asteroidBehaviour.isBigOne)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject smallAsteroid = asteroidPoolManager.GetAsteroid();

                    AsteroidBehaviour smallAsteroidBehaviour = smallAsteroid.GetComponent<AsteroidBehaviour>();
                    smallAsteroidBehaviour.RemoveBigForm();

                    smallAsteroid.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    smallAsteroid.transform.position = transform.position;
                    smallAsteroid.SetActive(true);
                }
                asteroidPoolManager.ReturnAsteroid(gameObject);
            }
            else
            {
                asteroidPoolManager.ReturnAsteroid(gameObject);
            }
        }

        public void SetPoolManager(AsteroidPoolManager manager)
        {
            asteroidPoolManager = manager;
        }
    }
}
