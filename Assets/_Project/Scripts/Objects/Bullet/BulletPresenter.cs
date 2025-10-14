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
        private SceneService _sceneService;

        public float CurrentLifetime { get; private set; }

        [Inject]
        public void Construct(BulletModel bm, SceneService ss)
        {
            _model = bm;
            _sceneService = ss;
        }

        public void Dispose()
        {
            if (_view != null)
            {

                _view.OnEnabled -= StartLifeTime;
                _view.OnLifeTimeSpendingFreeze -= SetCurrentLifeTime;
                _view.OnDeath -= HandleDeath;

                _sceneService.GameIsPaused -= PauseBullet;
                _sceneService.GameIsUnpaused -= UnpauseBullet;
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

            _sceneService.GameIsPaused += PauseBullet;
            _sceneService.GameIsUnpaused += UnpauseBullet;

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

        private void PauseBullet()
        {
            _view.TogglePause(true);
        }

        private void UnpauseBullet()
        {
            _view.TogglePause(false);
            _view.StartCoroutine(_view.LifeSpan(CurrentLifetime, _model.BulletsLifeTime));
        }

        private void SetCurrentLifeTime(float currentLifeTime)
        {
            CurrentLifetime = currentLifeTime;
        }
    }
}