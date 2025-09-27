using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class BulletPresenter : IHaveDeathConditions, IDisposable
    {
        public event Action<BulletView> OnDeath;

        private BulletModel _model;
        private BulletView _view;

        [Inject]
        public void Construct(BulletModel bm)
        {
            _model = bm;
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.OnHit -= HandleDeath;
                _view.OnEnabled -= Starter;
            }
        }

        public void Initialize(BulletView view)
        {
            _view = view;
            _view.OnHit += HandleDeath;
            _view.OnEnabled += Starter;
            _view.Initialize();
        }

        public async void StartLifeTime()
        {
            _view.MoveBullet(_model.MoveSpeed);
            await Task.Delay((int)(1000 * _model.BulletsLifeTime));
            if(_view != null)
            {
                HandleDeath();
            }
        }

        public void HandleDeath()
        {
            _view.StopBulletMovement();
            OnDeath?.Invoke(_view);
        }

        private void Starter()
        {
            StartLifeTime();
        }
    }
}