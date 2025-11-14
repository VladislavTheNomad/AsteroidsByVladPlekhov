using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class SaveData : IInitializable
    {
        private ScoreCounter _scoreCounter;
        private IAPService _iapService;
        private ProductList _productList;

        public bool HasUsedRevive { get; private set;}
        public bool HasAdBlock { get; private set; }


        [Inject]
        public void Construct(ScoreCounter sc, IAPService iAP, ProductList productList)
        { 
            _scoreCounter = sc;
            _iapService = iAP;
            _productList = productList;
        }

        public void Initialize()
        {
            HasUsedRevive = false;
            if (_iapService.Entitlements[_productList.GetAdBlockKey()] == true)
            {
                HasAdBlock = true;
                SaveAdBlock();
            }

            if (PlayerPrefs.HasKey("AdBlock"))
            {
                string hasAdBlock = PlayerPrefs.GetString("AdBlock");

                if (hasAdBlock == "true")
                {
                    HasAdBlock = true;
                }
            }
        }

        private void SaveAdBlock()
        {
            PlayerPrefs.SetString("AdBlock", "true");
        }

        public void SaveScore()
        {
            int currentScore = _scoreCounter.GetCurrentScore();

            if (PlayerPrefs.HasKey("BestScore"))
            {
                int currentBestScore = PlayerPrefs.GetInt("BestScore");

                if (currentBestScore >= currentScore) return;

                currentBestScore = currentScore;
                PlayerPrefs.SetInt("BestScore", currentBestScore);
            }
            else
            {
                PlayerPrefs.SetInt("BestScore", currentScore);
            }
        }

        public void UseRevive() => HasUsedRevive = true;
    }
}
