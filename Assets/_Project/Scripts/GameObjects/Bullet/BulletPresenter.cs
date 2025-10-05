using System;
using System.Threading.Tasks;
using Zenject;

namespace Asteroids
{
    public class BulletPresenter : IHaveDeathConditions, IDisposable
    {
        public event Action OnDeath;

        private BulletModel _model;
        private BulletView _view;
        private bool _isAlive = true;

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
            _isAlive = true;

            Starter();
        }

        public async void StartLifeTime()
        {
            _view.MoveBullet(_model.MoveSpeed);
            await Task.Delay((int)(1000 * _model.BulletsLifeTime));
            if (_view != null && _isAlive)
            {
                HandleDeath();
            }
        }

        public void HandleDeath()
        {
            if (!_isAlive) return;
            _isAlive = false;
            _view.StopBulletMovement();
            OnDeath?.Invoke();
        }

        private void Starter()
        {
            StartLifeTime();
        }
    }
}