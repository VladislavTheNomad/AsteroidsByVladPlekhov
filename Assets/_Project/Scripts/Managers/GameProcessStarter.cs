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
        private GameHUDManager _gameHUDManager;
        private bool _isPaused;

        [Inject]
        public void Construct(UtilsCalculatePositions utils, AsteroidFactory asteroidFactory, UfoFactory ufoFactory, GameHUDManager gameHUD)
        {
            _utilsMakeRandomStartPosition = utils;
            _asteroidFactory = asteroidFactory;
            _ufoFactory = ufoFactory;
            _gameHUDManager = gameHUD;
        }

        public void Initialize()
        {
            _asteroidSpawnDelay = new WaitForSeconds(_timeBetweenAsteroidsSpawns);
            _ufoSpawnDelay = new WaitForSeconds(_timeBetweenUFOSpawns);

            _gameHUDManager.OnGamePaused += StopGame;

            StartCoroutine(AsteroidsSpawn());
            StartCoroutine(UFOSpawn());
        }

        public void Dispose()
        {
            _gameHUDManager.OnGamePaused -= StopGame;
        }

        private void StopGame()
        {
            _isPaused = true;
        }

        private IEnumerator UFOSpawn()
        {
            yield return null;
            while (true && !_isPaused)
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
            while (true && !_isPaused)
            {
                    AsteroidView newAsteroid = _asteroidFactory.GetAsteroidFromPool();
                    newAsteroid.SetNewAsteroid();
                    yield return _asteroidSpawnDelay;
            }
        }
    }
}