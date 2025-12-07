using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class StoredDataHandler : IInitializable
    {
        public event Action OnChoosePlaceToSave;
        public event Action OnSaveScoreAfterDeath;

        private IStoreService _storeService;
        private IProductList _productList;
        private CloudData _cloudData;
        private int _newBestScore;

        public bool HasUsedRevive { get; private set; }
        public bool HasAdBlock { get; private set; }


        [Inject]
        public void Construct(IStoreService storeService, IProductList productList, CloudData cd)
        {
            _storeService = storeService;
            _productList = productList;
            _cloudData = cd;
        }

        public void Initialize()
        {
            HasUsedRevive = false;
        }

        public void CheckPurchasedProducts()
        {
            if (_storeService.CheckProductStatus(_productList.GetAdBlockKey()))
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

        public async Task SaveScoreAsync(int currentScore)
        {
            _newBestScore = currentScore;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SaveToLocal();
                OnSaveScoreAfterDeath?.Invoke();
            }
            else
            {
                (int score, string date) clodData = (0, "");
                clodData = await _cloudData.LoadDataFromCloud();
                string cloudScoreDate = clodData.date;
                DateTime.TryParseExact(
                    cloudScoreDate,
                    "yyyy:MM:dd - HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime resultCloud
                    );
                Debug.Log(resultCloud);

                string localScoreDate = PlayerPrefs.GetString("BestScoreDate");

                DateTime.TryParseExact(
                    localScoreDate,
                    "yyyy:MM:dd - HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime resultLocal
                    );

                Debug.Log(resultLocal);

                if (resultLocal >= resultCloud)
                {
                    OnChoosePlaceToSave?.Invoke();
                }
                else
                {
                    Debug.Log("Локальная дата не новее облачной");
                    SaveToCloud();
                    OnSaveScoreAfterDeath?.Invoke();
                }
            }
        }        

        public void UseRevive() => HasUsedRevive = true;

        public async Task<int> GetBestScoreAndUpdateData()
        {
            int bestFromCloud = 0;
            int bestFromLocal = 0;
            (int score, string date) cloudData = (0, "");

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                cloudData = await _cloudData.LoadDataFromCloud();
                bestFromCloud = cloudData.score;
            }

            if (PlayerPrefs.HasKey("AdBlock"))
            {
                bestFromLocal = PlayerPrefs.GetInt("BestScore");
            }

            int bestScore = bestFromCloud > bestFromLocal ? bestFromCloud : bestFromLocal;

            if (bestScore == bestFromLocal && Application.internetReachability != NetworkReachability.NotReachable)
            {
                SaveToCloud();
            }
            else if (bestScore == bestFromCloud && Application.internetReachability != NetworkReachability.NotReachable)
            {
                SaveToLocal();
            }

            _newBestScore = bestScore;
            return bestScore;
        }

        public void SaveToCloud()
        {
            _cloudData.SaveDataToCloud(_newBestScore);
        }

        public void SaveToLocal()
        {
            PlayerPrefs.SetInt("BestScore", _newBestScore);

            string date = DateTime.Now.ToString("yyyy:MM:dd - HH:mm:ss");
            PlayerPrefs.SetString("BestScoreDate", date);
        }
    }
}