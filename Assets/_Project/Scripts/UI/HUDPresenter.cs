using System;
using Zenject;

namespace Asteroids
{
    public class HUDPresenter : IDisposable
    {
        private HUDModel _model;
        private HUDView _view;

        [Inject]
        public void Construct(HUDModel model, HUDView view)
        {
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
            _model.OnPlayerDead += _view.ShowGameOverMenu;

            _model.OnScoreChanged += _view.UpdateScore;

            _view.OnRetryButtonClicked += _model.RequestReloadGame;
            _view.OnExitButtonClicked += _model.RequestExitGame;
        }

        public void Dispose()
        {
            _model.OnCoordinatesUpdated -= _view.UpdateCoordinates;
            _model.OnSpeedUpdated -= _view.UpdateSpeed;
            _model.OnCurrentShotsUpdated -= current => _view.UpdateCurrentShots(current, _model.MaxShots);
            _model.OnRechargeTimerUpdated -= _view.UpdateRechargeTimer;
            _model.OnPlayerDead -= _view.ShowGameOverMenu;

            _model.OnScoreChanged -= _view.UpdateScore;

            _view.OnRetryButtonClicked += _model.RequestReloadGame;
            _view.OnExitButtonClicked += _model.RequestExitGame;
        }
    }
}