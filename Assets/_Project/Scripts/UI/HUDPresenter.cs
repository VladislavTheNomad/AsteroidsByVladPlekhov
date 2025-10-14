using System;
using Zenject;

namespace Asteroids
{
    public class HUDPresenter : IDisposable
    {
        private PlayerPresenter _playerPresenter;
        private SceneService _sceneService;
        private HUDModel _model;
        private HUDView _view;
        private ScoreCounter _scoreCounter;

        [Inject]
        public void Construct(PlayerPresenter pp, SceneService ss, HUDModel model, HUDView view, ScoreCounter sc)
        {
            _playerPresenter = pp;
            _sceneService = ss;
            _scoreCounter = sc;
            _model = model;
            _view = view;
            RegisterDependencies();
        }

        public void RegisterDependencies()
        {
            _model.OnCoordinatesUpdated += _view.UpdateCoordinates;
            _model.OnSpeedUpdated += _view.UpdateSpeed;
            _model.OnCurrentShotsUpdated += current => _view.UpdateCurrentShots(current, _model.MaxShots);
            _model.OnRechargeTimerUpdated += _view.UpdateRechargeTimer;
            _model.OnPlayerDead += () =>
            {
                _view.ShowGameOverMenu();
                _sceneService.PauseGame();
            };

            _playerPresenter.OnPlayerIsDead += _model.PlayerDied;
            _playerPresenter.OnRechargeTimer += _model.UpdateRechargeTimer;

            _scoreCounter.OnScoreChanged += _view.UpdateScore;

            _view.OnRetryButtonClicked += _sceneService.ReloadGame;
            _view.OnExitButtonClicked += _sceneService.ExitGame;
        }

        public void Dispose()
        {
            _model.OnCoordinatesUpdated -= _view.UpdateCoordinates;
            _model.OnSpeedUpdated -= _view.UpdateSpeed;
            _model.OnCurrentShotsUpdated -= current => _view.UpdateCurrentShots(current, _model.MaxShots);
            _model.OnRechargeTimerUpdated -= _view.UpdateRechargeTimer;
            _model.OnPlayerDead -= () =>
            {
                _view.ShowGameOverMenu();
                _sceneService.PauseGame();
            };

            _playerPresenter.OnPlayerIsDead -= _model.PlayerDied;
            _playerPresenter.OnRechargeTimer -= _model.UpdateRechargeTimer;

            _scoreCounter.OnScoreChanged -= _view.UpdateScore;

            _view.OnRetryButtonClicked -= _sceneService.ReloadGame;
            _view.OnExitButtonClicked -= _sceneService.ExitGame;
        }
    }
}