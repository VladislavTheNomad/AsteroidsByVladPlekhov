using UnityEngine;

namespace Asteroids
{
    public class AsteroidSplitting : MonoBehaviour, IHaveDeathConditions
    {
        //connections
        [SerializeField] private AsteroidBehaviour asteroidBehaviour;
        private AsteroidPoolManager asteroidPoolManager;
        private UIManager uiManager;

        //own
        private static readonly Vector3 smallAsteroidScale = new Vector3(0.5f, 0.5f, 1f);


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

                    smallAsteroid.transform.localScale = smallAsteroidScale;
                    smallAsteroid.transform.position = transform.position;
                    smallAsteroid.SetActive(true);
                }
                DeathConditions();
            }
            else
            {
                DeathConditions();
            }
        }

        public void SetPoolManager(AsteroidPoolManager manager)
        {
            asteroidPoolManager = manager;
        }
        public void SetUIManager(UIManager UImanager)
        {
            uiManager = UImanager;
        }
        public void DeathConditions()
        {
            asteroidPoolManager.ReturnAsteroid(gameObject);
            uiManager.UpdateScore(1);
        }
    }
}
