using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidPoolManager : MonoBehaviour, IInitiable
    {
        //connections
        [SerializeField] private GameObject asteroidPrefab;
        [SerializeField] private int numOfAsteroidsInPool;
        [SerializeField] private UIManager uiManager;

        //own
        private List<GameObject> asteroidsPool;
        private Camera mainCamera;
        public int sortingIndex => 4;

        public void Installation()
        {
            mainCamera = Camera.main;
            asteroidsPool = new List<GameObject>(numOfAsteroidsInPool);
            for (int i = 0; i < numOfAsteroidsInPool; i++)
            {
                GameObject newAsteroid = Instantiate(asteroidPrefab);
                newAsteroid.SetActive(false);

                var asteroidScript = newAsteroid.GetComponent<AsteroidBehaviour>();
                var asteroidSplitting = newAsteroid.GetComponent<AsteroidSplitting>();
                asteroidScript.SetPoolManager(this);
                asteroidSplitting.SetPoolManager(this);
                asteroidSplitting.SetUIManager(uiManager);

                asteroidsPool.Add(newAsteroid);
            }
        }

        public GameObject GetAsteroid()
        {
            int count = asteroidsPool.Count;
            for (int i = 0; i < count; i++)
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
