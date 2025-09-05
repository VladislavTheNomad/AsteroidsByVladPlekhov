using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class GameProcessStarter : MonoBehaviour, IInitiable
    {

        [SerializeField, Range(1, 10)] private int _levelsOfAsteroidSplitting;
        [SerializeField] private float _timeBetweenAsteroidsSpawns;
        [SerializeField] private float _timeBetweenUFOSpawns;
        [SerializeField] private GamePoolsController _poolManager;
        [SerializeField] private UtilsCalculatePositions _utilsMakeRandomStartPosition;

        private WaitForSeconds _asteroidSpawnDelay;
        private WaitForSeconds _ufoSpawnDelay;

        public void Installation()
        {
            _asteroidSpawnDelay = new WaitForSeconds(_timeBetweenAsteroidsSpawns);
            _ufoSpawnDelay = new WaitForSeconds(_timeBetweenUFOSpawns);

            StartCoroutine(AsteroidsSpawn());
            StartCoroutine(UFOSpawn());
        }

        private IEnumerator UFOSpawn()
        {
            while (true)
            {
                yield return _ufoSpawnDelay;
                UfoBehaviour newUFO = _poolManager.GetUFOPool().Get();
                Vector2 spawnPosition = _utilsMakeRandomStartPosition.GetRandomSpawnPosition();
                newUFO.gameObject.transform.position = spawnPosition;
                newUFO.gameObject.SetActive(true);
            }
        }

        private IEnumerator AsteroidsSpawn()
        {
            while (true)
            {
                AsteroidBehaviour newAsteroid = _poolManager.GetAsteroidPool().Get();
                newAsteroid.SetMaxSizeLevel(_levelsOfAsteroidSplitting);
                newAsteroid.SetCurrentSizeLevel(_levelsOfAsteroidSplitting);
                Vector2 spawnPosition = _utilsMakeRandomStartPosition.GetRandomSpawnPosition();
                newAsteroid.gameObject.transform.position = spawnPosition;
                newAsteroid.gameObject.transform.localScale = Vector3.one;
                newAsteroid.gameObject.SetActive(true);
                yield return _asteroidSpawnDelay;
            }
        }
    }
}