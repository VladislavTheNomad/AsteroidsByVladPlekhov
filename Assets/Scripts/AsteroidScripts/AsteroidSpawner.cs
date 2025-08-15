using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour, IInitiable
    {
        public int sortingIndex => 5;
        private PlayerController player;

        //settings
        [SerializeField] private float timeBetweenSpawns;
        [SerializeField] private AsteroidPoolManager poolManager;

        public void Installation()
        {
            player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                StartCoroutine(Spawn());
            }
        }

        private IEnumerator Spawn()
        {
            while (player != null)
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
                newAsteroid.transform.localScale = new Vector3(1f, 1f, 1f);
                newAsteroid.SetActive(true);

            }
        }
    }
}
