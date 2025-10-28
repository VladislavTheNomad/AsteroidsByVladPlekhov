using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameProcessStarter : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private float _timeBetweenAsteroidsSpawns;
        [SerializeField] private float _timeBetweenUFOSpawns;

        private WaitForSeconds _asteroidSpawnDelay;
        private WaitForSeconds _ufoSpawnDelay;
        private UtilsCalculatePositions _utilsMakeRandomStartPosition;
        private AsteroidFactory _asteroidFactory;
        private UfoFactory _ufoFactory;
        private PauseGame _pauseManager;
        private IAnalytics _analytics;
        private bool _isPaused;

        [Inject]
        public void Construct(UtilsCalculatePositions utils, AsteroidFactory af, UfoFactory uf, PauseGame pm, IAnalytics analytics)
        {
            _utilsMakeRandomStartPosition = utils;
            _asteroidFactory = af;
            _ufoFactory = uf;
            _pauseManager = pm;
            _analytics = analytics;
        }

        public void Initialize()
        {
            _analytics.LogGameStart();

            _asteroidSpawnDelay = new WaitForSeconds(_timeBetweenAsteroidsSpawns);
            _ufoSpawnDelay = new WaitForSeconds(_timeBetweenUFOSpawns);

            _pauseManager.GameIsPaused += TogglePause;

            StartCoroutine(AsteroidsSpawn());
            StartCoroutine(UFOSpawn());
        }

        public void Dispose()
        {
            StopAllCoroutines();
            _pauseManager.GameIsPaused -= TogglePause;
        }

        private void TogglePause(bool condition)
        {
            _isPaused = condition;
        }

        private IEnumerator UFOSpawn()
        {
            yield return null;
            while (true)
            {
                yield return _ufoSpawnDelay;

                if(!_isPaused)
                {
                    UfoView newUFO = _ufoFactory.GetUfoFromPool();
                    Vector2 spawnPosition = _utilsMakeRandomStartPosition.GetRandomSpawnPosition();
                    newUFO.gameObject.transform.position = spawnPosition;
                }
            }
        }

        private IEnumerator AsteroidsSpawn()
        {
            yield return null;
            while (true)
            {
                yield return _asteroidSpawnDelay;
                
                if(!_isPaused)
                {
                    AsteroidView newAsteroid = _asteroidFactory.GetAsteroidFromPool();
                    newAsteroid.SetNewAsteroid();
                    yield return _asteroidSpawnDelay;
                }
            }
        }
    }
}