using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameProcessStarter : MonoBehaviour, IInitializable
    {
        [SerializeField, Range(1, 10)] private int _levelsOfAsteroidSplitting;
        [SerializeField] private float _timeBetweenAsteroidsSpawns;
        [SerializeField] private float _timeBetweenUFOSpawns;

        private WaitForSeconds _asteroidSpawnDelay;
        private WaitForSeconds _ufoSpawnDelay;
        private GamePoolsController _poolManager;
        private UtilsCalculatePositions _utilsMakeRandomStartPosition;

        [Inject]
        public void Construct(DiContainer container)
        {
            _utilsMakeRandomStartPosition = container.TryResolve<UtilsCalculatePositions>();
            _poolManager = container.TryResolve<GamePoolsController>();
        }

        public void Initialize()
        {
            _asteroidSpawnDelay = new WaitForSeconds(_timeBetweenAsteroidsSpawns);
            _ufoSpawnDelay = new WaitForSeconds(_timeBetweenUFOSpawns);

            StartCoroutine(AsteroidsSpawn());
            StartCoroutine(UFOSpawn());
        }

        private IEnumerator UFOSpawn()
        {
            yield return null;
            while (true)
            {
                yield return _ufoSpawnDelay;
                UfoView newUFO = _poolManager.GetUFOPool().Get();
                Vector2 spawnPosition = _utilsMakeRandomStartPosition.GetRandomSpawnPosition();
                newUFO.gameObject.transform.position = spawnPosition;
                newUFO.gameObject.SetActive(true);
            }
        }

        private IEnumerator AsteroidsSpawn()
        {
            yield return null;
            while (true)
            {
                AsteroidView newAsteroid = _poolManager.GetAsteroidPool().Get();
                newAsteroid.SetNewAsteroid();
                yield return _asteroidSpawnDelay;
            }
        }
    }
}