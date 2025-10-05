using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class BulletFactory
    {
        private readonly BulletPool _pool;
        private readonly DiContainer _container;

        [Inject]
        public BulletFactory(BulletPool pool, DiContainer container)
        {
            _pool = pool;
            _container = container;
        }

        public BulletView GetBulletFromPool(Transform playerTransform)
        {
            BulletView view = _pool.Spawn();
            view.transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
            BulletPresenter presenter = _container.Instantiate<BulletPresenter>();
            presenter.Initialize(view);
            Action OnDeathHandler = () => HandleBulletDeath(presenter, view);
            presenter.OnDeath += OnDeathHandler;
            view.gameObject.SetActive(true);
            return view;
        }

        private void HandleBulletDeath(BulletPresenter presenter, BulletView view)
        {
            presenter.OnDeath -= () => HandleBulletDeath(presenter, view);
            presenter.Dispose();
            _pool.Despawn(view);
        }
    }
}
