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
        private UtilsCalculatePositions _utilsMakeRandomStartPosition;
        private AsteroidFactory _asteroidFactory;
        private UfoFactory _ufoFactory;

        [Inject]
        public void Construct(UtilsCalculatePositions utils, AsteroidFactory asteroidFactory, UfoFactory ufoFactory)
        {
            _utilsMakeRandomStartPosition = utils;
            _asteroidFactory = asteroidFactory;
            _ufoFactory = ufoFactory;
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
                UfoView newUFO = _ufoFactory.GetUfoFromPool();
                Vector2 spawnPosition = _utilsMakeRandomStartPosition.GetRandomSpawnPosition();
                newUFO.gameObject.transform.position = spawnPosition;
            }
        }

        private IEnumerator AsteroidsSpawn()
        {
            yield return null;
            while (true)
            {
                AsteroidView newAsteroid = _asteroidFactory.GetAsteroidFromPool();
                newAsteroid.SetNewAsteroid();
                yield return _asteroidSpawnDelay;
            }
        }
    }
}