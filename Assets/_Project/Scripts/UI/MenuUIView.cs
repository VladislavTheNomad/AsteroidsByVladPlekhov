using System;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids
{
    public class MenuUIView : MonoBehaviour
    {
        public event Action OnGameStartClicked;
        public event Action OnBuyAdBlockClicked;

        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _buyAdBlockButton;

        private void OnEnable()
        {
            _gameStartButton.onClick.AddListener(() => OnGameStartClicked?.Invoke());
            _buyAdBlockButton.onClick.AddListener(() => OnBuyAdBlockClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _gameStartButton.onClick.RemoveAllListeners();
            _buyAdBlockButton.onClick.RemoveAllListeners();
        }
    }
}
