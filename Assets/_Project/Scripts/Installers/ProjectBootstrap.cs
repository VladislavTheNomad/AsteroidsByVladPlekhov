using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using System.Collections.Generic;

namespace Asteroids
{
    public class ProjectBootstrap : MonoBehaviour
    {
        private async void Start()
        {
            await InitializeFirebaseAndLoadConfigs();
            SceneManager.LoadScene("MainScene");
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

            Dictionary<string, object> defaults = new Dictionary<string, object>
            {
                { "AsteroidData", "{\"LaserRechargeTime\":10,\"BulletRechargeTime\":0.4,\"MovementSpeed\":10,\"RotationSpeed\":40,\"TimeBetweenAsteroidsSpawns\":3,\"TimeBetweenUFOSpawns\":3}" }
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