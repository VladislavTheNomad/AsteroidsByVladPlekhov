using System;
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

        public void Dispose()
        {
            _uiView.OnGameStartClicked -= _uiModel.StartGame;
            _uiView.OnBuyAdBlockClicked -= _uiModel.BuyAdBlock;
        }

        public void Initialize()
        {
            _uiView.OnGameStartClicked += _uiModel.StartGame;
            _uiView.OnBuyAdBlockClicked += _uiModel.BuyAdBlock;
        }
    }
}
