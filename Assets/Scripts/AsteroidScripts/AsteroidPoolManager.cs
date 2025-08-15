using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidPoolManager : MonoBehaviour, IInitiable
    {
        //connections
        [SerializeField] private GameObject asteroidPrefab;
        [SerializeField] private int numOfAsteroidsInPool;
        [SerializeField] private Camera mainCamera;

        private List<GameObject> asteroidsPool;
        public int sortingIndex => 4;

        public void Installation()
        {
            asteroidsPool = new List<GameObject>();
            for (int i = 0; i < numOfAsteroidsInPool; i++)
            {
                GameObject newAsteroid = Instantiate(asteroidPrefab);
                newAsteroid.SetActive(false);

                var asteroidScript = newAsteroid.GetComponent<AsteroidBehaviour>();
                var asteroidSlitting = newAsteroid.GetComponent<AsteroidSplitting>();
                asteroidScript.SetPoolManager(this);
                asteroidSlitting.SetPoolManager(this);

                asteroidsPool.Add(newAsteroid);
            }
        }

        public GameObject GetAsteroid()
        {
            for (int i = 0; i < asteroidsPool.Count; i++)
            {
                if (!asteroidsPool[i].activeSelf)
                {
                    return asteroidsPool[i];
                }
            }
            return null;
        }

        public void ReturnAsteroid(GameObject asteroid)
        {
            asteroid.SetActive(false);
        }

    }
}
