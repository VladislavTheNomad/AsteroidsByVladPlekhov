using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Asteroids
{
    public class MenuUIView : MonoBehaviour
    {
        public event Action OnGameStartClicked;
        public event Action OnBuyAdBlockClicked;
        public event Action OnReadyForDownloads;

        private UnityAction StartButtonClick;
        private UnityAction BuyAdblockButtonClick;

        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _buyAdBlockButton;

        private void OnEnable()
        {
            StartButtonClick = () => OnGameStartClicked?.Invoke();
            BuyAdblockButtonClick =  () => OnBuyAdBlockClicked?.Invoke();
        }

        private void Start()
        {
            _gameStartButton.onClick.AddListener(StartButtonClick);
            _buyAdBlockButton.onClick.AddListener(BuyAdblockButtonClick);

            _gameStartButton.interactable = false;
            OnReadyForDownloads?.Invoke();
        }

        private void OnDestroy()
        {
            _gameStartButton.onClick.RemoveAllListeners();
            _buyAdBlockButton.onClick.RemoveAllListeners();
        }

        public void GetAccessToStartButton()
        {
            _gameStartButton.interactable = true;
        }
    }
}
