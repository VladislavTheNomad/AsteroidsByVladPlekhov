using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class UfoPresenter : IGetPointsOnDestroy, IDisposable
    {
        public event Action<int> OnDeathTakeScore;
        public event Action OnDeath;

        private SceneService _sceneService;
        private UfoView _view;
        private UfoModel _model;
        private bool _initialized;

        [Inject]
        public void Construct(UfoModel model, SceneService sceneService)
        {
            _model = model;
            _sceneService = sceneService;
        }

        public void Initialize(UfoView view)
        {
            _view = view;
            _view.OnDeath += HandleDeath;
            _view.OnEnabled += Starter;
            _sceneService.GameIsPaused += PauseUfo;
            _sceneService.GameIsUnpaused += UnpauseUfo;
            _view.Initialize();
            _initialized = true;

            ApplyBehaviour();
        }

        public async void ApplyBehaviour()
        {
            while (_view.gameObject.activeSelf)
            {
                await Task.Delay((int)(_model.GapBetweenPositionChanging * 1000));
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

        public void HandleDeath()
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
                _sceneService.GameIsPaused -= PauseUfo;
                _sceneService.GameIsUnpaused -= UnpauseUfo;
            }
        }

        private void Starter()
        {
            if (_initialized)
            {
                ApplyBehaviour();
            }
        }

        private void UnpauseUfo()
        {
            _view.TogglePause(false);
        }

        private void PauseUfo()
        {
            _view.TogglePause(true);
        }
    }
}