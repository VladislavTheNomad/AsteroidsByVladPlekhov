using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using System.Collections.Generic;
using Zenject;

namespace Asteroids
{
    public class ProjectBootstrap : MonoBehaviour
    {
        private GameConfigs _gameConfigs;

        [Inject]
        public void Construct(GameConfigs gameConfigs)
        {
            _gameConfigs = gameConfigs;
        }

        private async void Start()
        {
            await InitializeFirebaseAndLoadConfigs();
            SceneManager.LoadScene("Menu");
        }

        private async Task InitializeFirebaseAndLoadConfigs()
        {

            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

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

        private async Task LoadRemoteConfigs()
        {
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;

            string json = JsonUtility.ToJson(_gameConfigs);

            Dictionary<string, object> defaults = new Dictionary<string, object>
            {
                { "AsteroidData", json }
            };

            await remoteConfig.SetDefaultsAsync(defaults);

            await remoteConfig.FetchAsync(System.TimeSpan.FromHours(0));

            if (remoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
            {
                await remoteConfig.ActivateAsync();
                Debug.Log("Remote Config activated.");
            }
            else
            {
                Debug.LogWarning("Remote Config activation fails.");
            }
        }
    }
}