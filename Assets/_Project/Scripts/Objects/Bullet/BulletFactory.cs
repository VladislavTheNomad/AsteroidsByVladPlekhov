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
            view.gameObject.SetActive(true);
            view.transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
            BulletPresenter presenter = _container.Instantiate<BulletPresenter>();
            presenter.Initialize(view);

            Action handler = null;
            handler = () => HandleBulletDeath(presenter, view, handler);
            presenter.OnDeath += handler;

            return view;
        }

        private void HandleBulletDeath(BulletPresenter presenter, BulletView view, Action handler)
        {
            presenter.OnDeath -= handler;
            presenter.Dispose();
            _pool.Despawn(view);
        }
    }
}
