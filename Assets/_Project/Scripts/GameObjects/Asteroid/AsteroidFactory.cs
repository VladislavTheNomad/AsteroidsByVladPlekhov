using System;
using Zenject;

namespace Asteroids
{
    public class AsteroidFactory
    {

        private readonly AsteroidPool _pool;
        private readonly DiContainer _container;
        private readonly GameHUDManager _uiManager;

        [Inject]
        public AsteroidFactory(AsteroidPool pool, DiContainer container, GameHUDManager manager)
        {
            _pool = pool;
            _container = container;
            _uiManager = manager;
        }

        public AsteroidView GetAsteroidFromPool()
        {
            AsteroidView view = _pool.Spawn();
            AsteroidPresenter presenter = _container.Instantiate<AsteroidPresenter>();
            presenter.Initialize(view);
            _uiManager.SubscribeOnDeath(presenter);
            Action OnDeathHandler = () => HandleAsteroidDeath(presenter, view);
            presenter.OnDeath += OnDeathHandler;

            return view;
        }

        private void HandleAsteroidDeath(AsteroidPresenter presenter, AsteroidView view)
        {
            _uiManager.UnsubscribeOnDeath(presenter);
            presenter.OnDeath -= () => HandleAsteroidDeath(presenter, view);
            presenter.Dispose();
            _pool.Despawn(view);
        }
    }
}
