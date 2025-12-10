using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Asteroids
{
    public class MenuUiModel
    {
        public event Action OnDownloadRemoteDataCompleted;
        
        private IStoreService _iapService;
        private IAPProductList _productList;
        private string _labelToDownload = "default";
        private AsyncOperationHandle _downloadHandle;

        [Inject]
        public void Construct(IStoreService iapService, IAPProductList productList)
        {
            _iapService = iapService;
            _productList = productList;
        }

        public async Task StartDownloadRemoteAddressables()
        {
            await Addressables.InitializeAsync().Task;

            _downloadHandle = Addressables.DownloadDependenciesAsync(_labelToDownload);

            try
            {
                await _downloadHandle.Task;

                if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Remote Addressables successfully downloaded!");
                    OnDownloadRemoteDataCompleted?.Invoke();
                }
                else
                {
                    Debug.Log($"Failed to download remote data: {_downloadHandle.OperationException}");
                }
            }
            finally
            {
                Addressables.Release(_downloadHandle);
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        internal void BuyAdBlock()
        {
            _iapService.BuyProduct(_productList.GetAdBlockKey());
        }

        
    }
}
