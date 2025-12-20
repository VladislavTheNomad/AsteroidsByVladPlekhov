using System;
using UnityEngine;
using Firebase;
using Firebase.RemoteConfig;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Asteroids
{
    public class ProjectBootstrap : IInitializable
    {
        private GameConfigs _gameConfigs;
        private CloudData _cloudData;
        private SceneService _sceneService;

        [Inject]
        public ProjectBootstrap(GameConfigs gameConfigs, CloudData cloudData, SceneService sceneService)
        {
            _gameConfigs = gameConfigs;
            _cloudData =  cloudData;
            _sceneService =  sceneService;
        }
        
        public void Initialize()
        {
            StartTheGame().Forget();
        }

        private async UniTaskVoid StartTheGame()
        {
            try
            {
                await UniTask.WhenAll(_cloudData.InitCloudDataAsync(), InitializeFirebaseAndLoadConfigs());
                _sceneService.OpenMenu();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Bootstrap failed: {ex.Message}");
                _sceneService.OpenMenu();
            }
        }

        private async UniTask InitializeFirebaseAndLoadConfigs()
        {

            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (status == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                await LoadRemoteConfigs();
            }
            else
            {
                Debug.LogError("Error Firebase: " + status);
            }
        }

        private async UniTask LoadRemoteConfigs()
        {
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;

            string json = JsonUtility.ToJson(_gameConfigs);

            Dictionary<string, object> defaults = new Dictionary<string, object>
            {
                { "AsteroidData", json }
            };

            await remoteConfig.SetDefaultsAsync(defaults).AsUniTask();

            await remoteConfig.FetchAsync(System.TimeSpan.FromHours(0)).AsUniTask();

            if (remoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
            {
                await remoteConfig.ActivateAsync().AsUniTask();
                Debug.Log("Remote Config activated.");
            }
            else
            {
                Debug.LogWarning("Remote Config activation fails.");
            }
        }
    }
}