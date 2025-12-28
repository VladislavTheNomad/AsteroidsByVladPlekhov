using System;
using UnityEngine.Events;
using Zenject;

namespace Asteroids
{
    public class Statistics : IInitializable, IDisposable
    {
        public event Action OnLaserUsed;
        public event Action OnGameEnd;

        private Action _laserFiredAction;
        private Action _gameEndAction;
        
        private PlayerModel _playerModel;
        private UfoFactory _ufoFactory;
        private AsteroidFactory _asteroidFactory;
        private SceneService _sceneService;

        public int BulletsFired { get; private set; }
        public int LasersFired { get; private set; }
        public int UfosKilled { get; private set; }
        public int AsteroidsKilled { get; private set; }

        [Inject]
        public Statistics(UfoFactory ufoFactory, AsteroidFactory asteroidFactory, SceneService sceneService, PlayerModel playerModel)
        {
            _ufoFactory = ufoFactory;
            _asteroidFactory = asteroidFactory;
            _sceneService = sceneService;
            _playerModel = playerModel;
        }
        
        public void Initialize()
        {
            _laserFiredAction = () => OnLaserUsed?.Invoke();
            _gameEndAction = () => OnGameEnd?.Invoke();
            
            _ufoFactory.OnReturnToPool += UfoKilledStat;
            _asteroidFactory.OnReturnToPool += AsteroidKilledStat;
            _playerModel.BulletFired += UpdateBulletFiredStat;
            _playerModel.LaserFired += UpdateLaserFiredStat;
            _playerModel.LaserFired += _laserFiredAction;
            _sceneService.OnExitGame += _gameEndAction;
        }

        private void UpdateBulletFiredStat() => BulletsFired += 1;
        private void UpdateLaserFiredStat() => LasersFired += 1;
        private void UfoKilledStat() => UfosKilled += 1;
        private void AsteroidKilledStat() => AsteroidsKilled += 1;

        public void Dispose()
        {
            _playerModel.BulletFired -= UpdateBulletFiredStat;
            _playerModel.LaserFired -= UpdateLaserFiredStat;
            _ufoFactory.OnReturnToPool -= UfoKilledStat;
            _asteroidFactory.OnReturnToPool -= AsteroidKilledStat;
            _playerModel.LaserFired -= _laserFiredAction;
            _sceneService.OnExitGame -= _gameEndAction;
        }
    }
}
