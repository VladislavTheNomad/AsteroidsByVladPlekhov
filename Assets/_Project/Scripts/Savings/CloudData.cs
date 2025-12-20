using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

namespace Asteroids
{

    public class CloudData
    {
     
        private StoredDataNames _storedDataNames;
        private bool _isInitialized;

        [Inject]
        public void Construct(StoredDataNames storedDataNames)
        {
            _storedDataNames =  storedDataNames;
        }
        
        public async UniTask InitCloudDataAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            try
            {
                await UnityServices.InitializeAsync().AsUniTask();
                await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();
                Debug.Log("Cloud Services Initialized");
            }
            catch (Exception ex)
            {
                _isInitialized =  false;
                Debug.Log("Cloud Services failed to initialize " + ex);
            }
        }

        public async UniTask SaveDataToCloud(DataToSave data)
        {
            var dataForCloud = new Dictionary<string, object>
            {
                { _storedDataNames.DATA_NAME, data }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(dataForCloud).AsUniTask();
            Debug.Log($"Saved Data: {_storedDataNames.DATA_NAME}");
        }


        public async UniTask<DataToSave> LoadDataFromCloud()
        {
            var key = new HashSet<string> { _storedDataNames.DATA_NAME };
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(key).AsUniTask();

            if (playerData.TryGetValue(_storedDataNames.DATA_NAME, out var data))
            {
                return data.Value.GetAs<DataToSave>();
            }
            else
            {
                Debug.Log("Fail to Load Data from Cloud. Created the new Data storage.");
                return new DataToSave();
            }
        }
    }
}
