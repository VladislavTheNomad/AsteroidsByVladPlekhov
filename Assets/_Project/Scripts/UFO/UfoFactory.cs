using Zenject;

namespace Asteroids
{
    public class UfoFactory
    {
        private readonly UfoPool _pool;
        private readonly DiContainer _container;
        private readonly UIManager _uiManager;

        [Inject]
        public UfoFactory(UfoPool pool, DiContainer container, UIManager manager)
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
                presenter.Dispose();
                _pool.Despawn(view);
            };

            return view;
        }
    }
}
