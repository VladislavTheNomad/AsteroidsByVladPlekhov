using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class UfoPresenter : IGetPointsOnDestroy, IDisposable
    {
        public event Action<int> OnDeathTakeScore;
        public event Action OnDeath;

        private UfoView _view;
        private UfoModel _model;
        private bool _initialized;

        [Inject]
        public void Construct(UfoModel model)
        {
            _model = model;
        }

        public void Initialize(UfoView view)
        {
            _view = view;
            _view.OnDeath += HandleDeath;
            _view.OnEnabled += Starter;
            _model.IsGamePaused += PauseUfo;
            _view.Initialize();
            _initialized = true;

            ApplyBehaviour().Forget();
        }

        private async UniTaskVoid ApplyBehaviour()
        {
            var ct = _view.GetCancellationTokenOnDestroy();
            
            try
            {
                while (_view.gameObject.activeSelf)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_model.GapBetweenPositionChanging), cancellationToken: ct);
                    if (_model.CheckNull(_view.ViewTransform))
                    {
                        _model.GetNewDestination(_view.ViewTransform, out Vector3 destination, out float speed);
                        _view.Move(destination, speed);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private void HandleDeath()
        {
            OnDeathTakeScore?.Invoke(_model.ScorePoints);
            OnDeath?.Invoke();
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.OnDeath -= HandleDeath;
                _view.OnEnabled -= Starter;
                _model.IsGamePaused -= PauseUfo;
            }
        }

        private void Starter()
        {
            if (_initialized)
            {
                ApplyBehaviour();
            }
        }
        private void PauseUfo(bool condition)
        {
            _view.TogglePause(condition);
        }
    }
}