using System;
using Zenject;

namespace Asteroids
{
    public class UfoFactory
    {
        private readonly MonoMemoryPool<UfoView> _pool;
        private readonly DiContainer _container;
        private readonly ScoreCounter _scoreCounter;

        [Inject]
        public UfoFactory(MonoMemoryPool<UfoView> pool, DiContainer container, ScoreCounter scoreCounter)
        {
            _pool = pool;
            _container = container;
            _scoreCounter = scoreCounter;
        }

        public UfoView GetUfoFromPool()
        {
            UfoView view = _pool.Spawn();
            UfoPresenter presenter = _container.Instantiate<UfoPresenter>();
            presenter.Initialize(view);
            _scoreCounter.SubscribeOnDeath(presenter);
            Action OnDeathHandler = () => HandlerUfoDeath(presenter, view);
            presenter.OnDeath += OnDeathHandler;

            return view;
        }

        private void HandlerUfoDeath(UfoPresenter presenter, UfoView view)
        {
            _scoreCounter.UnsubscribeOnDeath(presenter);
            presenter.OnDeath -= () => HandlerUfoDeath(presenter, view);
            presenter.Dispose();
            _pool.Despawn(view);
        }
    }
}
