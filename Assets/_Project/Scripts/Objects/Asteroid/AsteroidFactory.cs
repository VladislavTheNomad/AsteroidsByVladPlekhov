using System;
using Zenject;

namespace Asteroids
{
    public class AsteroidFactory
    {
        public Action OnReturnToPool;

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

            Action handler = null;
            handler = () => HandleAsteroidDeath(presenter, view, handler);
            presenter.OnDeath += handler;


            return view;
        }

        private void HandleAsteroidDeath(AsteroidPresenter presenter, AsteroidView view, Action handler)
        {
            OnReturnToPool?.Invoke();
            _scoreCounter.UnsubscribeOnDeath(presenter);
            presenter.OnDeath -= handler;
            presenter.Dispose();
            _pool.Despawn(view);
        }
    }
}
