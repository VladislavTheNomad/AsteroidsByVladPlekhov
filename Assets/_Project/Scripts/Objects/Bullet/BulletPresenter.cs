using System;
using Zenject;

namespace Asteroids
{
    public class BulletPresenter : IHaveDeathConditions, IDisposable
    {
        public event Action OnDeath;

        private BulletModel _model;
        private BulletView _view;
        private bool _isAlive = true;
        private PauseGame _pauseManager;

        public float CurrentLifetime { get; private set; }

        [Inject]
        public void Construct(BulletModel bm, PauseGame pm)
        {
            _model = bm;
            _pauseManager = pm;
        }

        public void Dispose()
        {
            if (_view != null)
            {

                _view.OnEnabled -= StartLifeTime;
                _view.OnLifeTimeSpendingFreeze -= SetCurrentLifeTime;
                _view.OnDeath -= HandleDeath;

                _pauseManager.GameIsPaused -= PauseBullet;
            }
        }

        public void Initialize(BulletView view)
        {
            _view = view;
            _view.OnEnabled += StartLifeTime;
            _view.OnLifeTimeSpendingFreeze += SetCurrentLifeTime;
            _view.OnDeath += HandleDeath;
            _view.Initialize();
            _isAlive = true;

            _pauseManager.GameIsPaused += PauseBullet;

            SetCurrentLifeTime(0f);
            StartLifeTime();
        }

        public void StartLifeTime()
        {
            _view.MoveBullet(_model.MoveSpeed);
            _view.StartCoroutine(_view.LifeSpan(CurrentLifetime, _model.BulletsLifeTime));
        }

        public void HandleDeath()
        {
            if (!_isAlive) return;
            _isAlive = false;
            _view.StopBulletMovement();
            OnDeath?.Invoke();
        }

        private void PauseBullet(bool condition)
        {
            switch (condition)
            {
                case true:
                    _view.TogglePause(true);
                    break;
                case false:
                    _view.TogglePause(false);
                    _view.StartCoroutine(_view.LifeSpan(CurrentLifetime, _model.BulletsLifeTime));
                    break;
            }
        }

        private void SetCurrentLifeTime(float currentLifeTime)
        {
            CurrentLifetime = currentLifeTime;
        }
    }
}