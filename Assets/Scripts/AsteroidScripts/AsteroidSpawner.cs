using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour, IInitiable
    {
        //own
        public int sortingIndex => 5;

        //settings
        [SerializeField] private float timeBetweenSpawns;
        [SerializeField] private AsteroidPoolManager poolManager;
        [SerializeField] private PlayerController player;

        public void Installation()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeBetweenSpawns);
                GameObject newAsteroid = poolManager.GetAsteroid();
                if (newAsteroid == null) continue;

                Vector2 spawnPosition = UtilsMakeRandomStartPosition.MakeRandomStartPosition();
                newAsteroid.transform.position = spawnPosition;

                AsteroidBehaviour asteroidBehaviour = newAsteroid.GetComponent<AsteroidBehaviour>();
                if (asteroidBehaviour != null )
                {
                    asteroidBehaviour.SetBigForm();
                }
                newAsteroid.transform.localScale = Vector3.one;
                newAsteroid.SetActive(true);
            }
        }
    }
}
