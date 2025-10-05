using Zenject;

namespace Asteroids
{
    public class UfoFactory
    {
        private readonly UfoPool _pool;
        private readonly DiContainer _container;
        private readonly GameHUDManager _uiManager;

        [Inject]
        public UfoFactory(UfoPool pool, DiContainer container, GameHUDManager manager)
        {
            _pool = pool;
            _container = container;
            _uiManager = manager;
        }

        public UfoView GetUfoFromPool()
        {
            UfoView view = _pool.Spawn();
            UfoPresenter presenter = _container.Instantiate<UfoPresenter>();
            presenter.Initialize(view);
            _uiManager.SubscribeOnDeath(presenter);
            presenter.OnDeath += () =>
            {
                _uiManager.UnsubscribeOnDeath(presenter);
                presenter.Dispose();
                _pool.Despawn(view);
            };

            return view;
        }
    }
}
