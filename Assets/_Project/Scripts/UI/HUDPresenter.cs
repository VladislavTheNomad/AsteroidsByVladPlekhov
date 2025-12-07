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
            _model.OnRevive += _view.HideGameOverMenu;
            _model.OnBestScoreSetup += _view.UpdateBestScore;
            _model.OnNewRecord += _view.ShowNewRecordUI;

            _view.OnRewardedButtonClicked += _model.RequestRewardedAd;
            _view.OnRewardedButtonClicked += _view.HideRewardButton;
            _view.OnRetryButtonClicked += _model.RequestReloadGame;
            _view.OnExitButtonClicked += _model.RequestExitGame;

            _view.OnSaveToLocalClicked += _model.SaveScoreToLocal;
            _view.OnSaveToLocalClicked += _view.HideNewRecordUI;
            _view.OnSaveToLocalClicked += _view.ShowGameOverMenu;
            _view.OnSaveToCloudClicked += _model.SaveScoreToCloud;
            _view.OnSaveToCloudClicked += _view.HideNewRecordUI;
            _view.OnSaveToCloudClicked += _view.ShowGameOverMenu;
        }

        public void Dispose()
        {
            _model.OnCoordinatesUpdated -= _view.UpdateCoordinates;
            _model.OnSpeedUpdated -= _view.UpdateSpeed;
            _model.OnCurrentShotsUpdated -= current => _view.UpdateCurrentShots(current, _model.MaxShots);
            _model.OnRechargeTimerUpdated -= _view.UpdateRechargeTimer;
            _model.OnPlayerDead -= _view.ShowGameOverMenu;
            _model.OnScoreChanged -= _view.UpdateScore;
            _model.OnRevive -= _view.HideGameOverMenu;
            _model.OnBestScoreSetup -= _view.UpdateBestScore;
            _model.OnNewRecord -= _view.ShowNewRecordUI;

            _view.OnRewardedButtonClicked -= _model.RequestRewardedAd;
            _view.OnRewardedButtonClicked -= _view.HideRewardButton;
            _view.OnRetryButtonClicked -= _model.RequestReloadGame;
            _view.OnExitButtonClicked -= _model.RequestExitGame;

            _view.OnSaveToLocalClicked -= _model.SaveScoreToLocal;
            _view.OnSaveToLocalClicked -= _view.HideNewRecordUI;
            _view.OnSaveToLocalClicked -= _view.ShowGameOverMenu;
            _view.OnSaveToCloudClicked -= _model.SaveScoreToCloud;
            _view.OnSaveToCloudClicked -= _view.HideNewRecordUI;
            _view.OnSaveToCloudClicked -= _view.ShowGameOverMenu;
        }
    }
}