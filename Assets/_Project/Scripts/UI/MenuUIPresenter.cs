using System;
using Zenject;

namespace Asteroids
{
    public class MenuUIPresenter : IInitializable, IDisposable
    {
        private MenuUIView _uiView;
        private MenuUiModel _uiModel;

        [Inject]
        public void Construct(MenuUIView view, MenuUiModel model)
        {
            _uiView = view;
            _uiModel = model;
        }
        
        public void Initialize()
        {
            _uiView.OnGameStartClicked += _uiModel.StartGame;
            _uiView.OnBuyAdBlockClicked += _uiModel.BuyAdBlock;
            _uiView.OnReadyForDownloads += OnReadyForDownloadsAsync;

            _uiModel.OnDownloadRemoteDataCompleted += _uiView.GetAccessToStartButton;
        }

        public void Dispose()
        {
            _uiView.OnGameStartClicked -= _uiModel.StartGame;
            _uiView.OnBuyAdBlockClicked -= _uiModel.BuyAdBlock;
            _uiView.OnReadyForDownloads -= OnReadyForDownloadsAsync;
            
            _uiModel.OnDownloadRemoteDataCompleted -= _uiView.GetAccessToStartButton;
        }
        
        private async void OnReadyForDownloadsAsync()
        {
            await _uiModel.StartDownloadRemoteAddressables();
        }
    }
}
