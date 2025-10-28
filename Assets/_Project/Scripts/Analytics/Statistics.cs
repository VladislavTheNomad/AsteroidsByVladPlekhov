using System;
using Zenject;

namespace Asteroids
{
    public class Statistics : IDisposable
    {
        private PlayerModel _playerModel;
        private UfoFactory _ufoFactory;
        private AsteroidFactory _asteroidFactory;
        private SceneService _sceneService;
        private IAnalytics _analytics;

        public int BulletsFired { get; private set; }
        public int LasersFired { get; private set; }
        public int UfosKilled { get; private set; }
        public int AsteroidsKilled { get; private set; }

        [Inject]
        public Statistics(UfoFactory uf, AsteroidFactory af, SceneService ss)
        {
            _ufoFactory = uf;
            _asteroidFactory = af;
            _sceneService = ss;

            _ufoFactory.OnReturnToPool += UfoKilledStat;
            _asteroidFactory.OnReturnToPool += AsteroidKilledStat;
        }

        [Inject]
        public void PostConstruct(PlayerModel pm, IAnalytics analytics)
        {
            _playerModel = pm;
            _analytics = analytics;

            _playerModel.BulletFired += UpdateBulletFiredStat;
            _playerModel.LaserFired += UpdateLaserFiredStat;
            _playerModel.LaserFired += _analytics.LogLaserUsed;
            _sceneService.OnExitGame += _analytics.LogGameEnd;
        }

        private void UpdateBulletFiredStat() => BulletsFired += 1;
        private void UpdateLaserFiredStat() => LasersFired += 1;
        private void UfoKilledStat() => UfosKilled += 1;
        private void AsteroidKilledStat() => AsteroidsKilled += 1;

        public void Dispose()
        {
            _sceneService.OnExitGame -= _analytics.LogGameEnd;
            _playerModel.BulletFired -= UpdateBulletFiredStat;
            _playerModel.LaserFired -= UpdateLaserFiredStat;
            _ufoFactory.OnReturnToPool -= UfoKilledStat;
            _asteroidFactory.OnReturnToPool -= AsteroidKilledStat;
        }
    }
}
