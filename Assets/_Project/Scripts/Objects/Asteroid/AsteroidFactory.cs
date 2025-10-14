using System;
using Zenject;

namespace Asteroids
{
    public class AsteroidFactory
    {

        private readonly MonoMemoryPool<AsteroidView> _pool;
        private readonly DiContainer _container;
        private readonly ScoreCounter _scoreCounter;

        [Inject]
        public AsteroidFactory(MonoMemoryPool<AsteroidView> pool, DiContainer container, ScoreCounter scoreCounter)
        {
            _pool = pool;
            _container = container;
            _scoreCounter = scoreCounter;
        }

        public AsteroidView GetAsteroidFromPool()
        {
            AsteroidView view = _pool.Spawn();
            AsteroidPresenter presenter = _container.Instantiate<AsteroidPresenter>();
            presenter.Initialize(view);
            _scoreCounter.SubscribeOnDeath(presenter);
            Action OnDeathHandler = () => HandleAsteroidDeath(presenter, view);
            presenter.OnDeath += OnDeathHandler;

            return view;
        }

        private void HandleAsteroidDeath(AsteroidPresenter presenter, AsteroidView view)
        {
            _scoreCounter.UnsubscribeOnDeath(presenter);
            presenter.OnDeath -= () => HandleAsteroidDeath(presenter, view);
            presenter.Dispose();
            _pool.Despawn(view);
        }
    }
}
