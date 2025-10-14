using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private SceneService _sceneService;
        private bool _isPaused;

        [Inject]
        public void Construct(UtilsCalculatePositions utils, AsteroidFactory af, UfoFactory uf, SceneService ss)
        {
            _utilsMakeRandomStartPosition = utils;
            _asteroidFactory = af;
            _ufoFactory = uf;
            _sceneService = ss;
        }

        public void Initialize()
        {
            _asteroidSpawnDelay = new WaitForSeconds(_timeBetweenAsteroidsSpawns);
            _ufoSpawnDelay = new WaitForSeconds(_timeBetweenUFOSpawns);

            _sceneService.GameIsPaused += () => _isPaused = true;
            _sceneService.GameIsUnpaused += () => _isPaused = false;

            StartCoroutine(AsteroidsSpawn());
            StartCoroutine(UFOSpawn());
        }

        public void Dispose()
        {
            StopAllCoroutines();
            _sceneService.GameIsPaused -= () => _isPaused = true;
            _sceneService.GameIsUnpaused -= () => _isPaused = false;
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