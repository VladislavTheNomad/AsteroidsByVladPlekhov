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
            AsteroidView asteroidView = _pool.Spawn();
            AsteroidPresenter presenter = _container.Instantiate<AsteroidPresenter>();
            presenter.Initialize(asteroidView);
            _uiManager.SubscribeOnDeath(presenter);
            presenter.OnDeath += () =>
            {
                _uiManager.UnsubscribeOnDeath(presenter);
                presenter.Dispose();
                _pool.Despawn(asteroidView);
            };

            return asteroidView;
        }

    }
}
