using System;
using UnityEngine.SceneManagement;
using Zenject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace Asteroids
{
    public class MenuUiModel
    {
        public event Action OnDownloadRemoteDataCompleted;
        
        private IStoreService _storeService;
        private IAPProductList _productList;
        private GlobalAssetCache _globalAssetCache;
        private AsyncOperationHandle _downloadHandle;
        private string _labelToDownload = "default";

        [Inject]
        public void Construct(IStoreService storeService, IAPProductList productList, GlobalAssetCache globalAssetCache)
        {
            _storeService = storeService;
            _productList = productList;
            _globalAssetCache = globalAssetCache;
        }

        private async UniTask StartDownloadRemoteAddressables()
        {
            await Addressables.InitializeAsync().ToUniTask();

            _downloadHandle = Addressables.DownloadDependenciesAsync(_labelToDownload);

            try
            {
                await _downloadHandle.ToUniTask();

                if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Remote Addressables successfully downloaded!");
                }
                else
                {
                    Debug.Log($"Failed to download remote data: {_downloadHandle.OperationException}");
                }
            }
            finally
            {
                if (_downloadHandle.IsValid())
                {
                    Addressables.Release(_downloadHandle);
                }
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        public void BuyAdBlock()
        {
            _storeService.BuyProduct(_productList.GetAdBlockKey());
        }

        public void OnReadyForDownloads()
        {
            LoadAndPrepareGameAssetsAsync().Forget();
        }
        
        private async UniTaskVoid LoadAndPrepareGameAssetsAsync()
        {
            try
            {
                await StartDownloadRemoteAddressables();
                await _globalAssetCache.LoadGameAssetsAsync();
                OnDownloadRemoteDataCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Fatal Error during Asset Loading: {ex}");
            }
        }
    }
}
