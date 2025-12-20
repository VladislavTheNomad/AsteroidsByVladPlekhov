using System;
using System.Threading.Tasks;
using UnityEngine;
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
            _uiView.OnReadyForDownloads += _uiModel.OnReadyForDownloads;
            
            _uiModel.OnDownloadRemoteDataCompleted += _uiView.GetAccessToStartButton;
        }

        public void Dispose()
        {
            _uiView.OnGameStartClicked -= _uiModel.StartGame;
            _uiView.OnBuyAdBlockClicked -= _uiModel.BuyAdBlock;
            _uiView.OnReadyForDownloads -= _uiModel.OnReadyForDownloads;
            
            _uiModel.OnDownloadRemoteDataCompleted -= _uiView.GetAccessToStartButton;
        }

    }
}
