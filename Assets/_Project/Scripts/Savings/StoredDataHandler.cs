using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
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
        private StoredDataNames _storedDataNames;
        private DataToSave _currentData;
        private int _newBestScore;

        public bool HasUsedRevive { get; private set; }


        [Inject]
        public void Construct(IStoreService storeService, IProductList productList, CloudData cd, StoredDataNames storedDataNames, DataToSave currentData)
        {
            _storeService = storeService;
            _productList = productList;
            _cloudData = cd;
            _storedDataNames = storedDataNames;
            _currentData = currentData;
        }

        public void Initialize()
        {
            HasUsedRevive = false;
        }

        public void CheckPurchasedProducts()
        {
            if (_storeService.CheckProductStatus(_productList.GetAdBlockKey()))
            {
                _currentData._hasAdBlock =  true;
            }
        }

        public async UniTask SaveScoreAsync(int currentScore)
        {
            _newBestScore = currentScore;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SaveToLocal();
                OnSaveScoreAfterDeath?.Invoke();
            }
            else
            {
                var cloudData = await _cloudData.LoadDataFromCloud();
                string cloudScoreDate = cloudData._saveDate;
                DateTime.TryParseExact(
                    cloudScoreDate,
                    "yyyy:MM:dd - HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime resultCloud
                    );
                Debug.Log(resultCloud);

                string localScoreDate = _currentData._saveDate;

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
                    await SaveToCloudAsync();
                    OnSaveScoreAfterDeath?.Invoke();
                }
            }
        }        

        public void UseRevive() => HasUsedRevive = true;

        public async UniTask<int> GetBestScoreAndUpdateData()
        {
            int bestFromCloud = 0;
            int bestFromLocal = 0;

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                var cloudData = await _cloudData.LoadDataFromCloud();
                bestFromCloud = cloudData._bestScore;
            }

            if (PlayerPrefs.HasKey(_storedDataNames.DATA_NAME))
            {
                string dataString = PlayerPrefs.GetString(_storedDataNames.DATA_NAME);
                if (!string.IsNullOrEmpty(dataString))
                {
                    _currentData = JsonUtility.FromJson<DataToSave>(dataString);
                    bestFromLocal = _currentData._bestScore;
                }
                else
                {
                    Debug.LogWarning($"Data {_storedDataNames.DATA_NAME} could not be loaded.");
                }
            }

            int bestScore = bestFromCloud > bestFromLocal ? bestFromCloud : bestFromLocal;

            if (bestScore == bestFromLocal && Application.internetReachability != NetworkReachability.NotReachable)
            {
                await SaveToCloudAsync();
            }
            else if (bestScore == bestFromCloud && Application.internetReachability != NetworkReachability.NotReachable)
            {
                SaveToLocal();
            }

            _newBestScore = bestScore;
            return bestScore;
        }

        public async UniTask SaveToCloudAsync()
        {
            _currentData._bestScore = _newBestScore;
            _currentData._saveDate = DateTime.Now.ToString("yyyy:MM:dd - HH:mm:ss");
            await _cloudData.SaveDataToCloud(_currentData);
        }

        public void SaveToLocal()
        {
            _currentData._bestScore = _newBestScore;
            _currentData._saveDate = DateTime.Now.ToString("yyyy:MM:dd - HH:mm:ss");
            string json = JsonUtility.ToJson(_currentData);
            PlayerPrefs.SetString(_storedDataNames.DATA_NAME, json);
            PlayerPrefs.Save();
        }
    }
}