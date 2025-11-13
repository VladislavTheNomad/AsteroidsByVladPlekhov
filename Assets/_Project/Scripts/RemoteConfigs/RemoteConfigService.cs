using Firebase.RemoteConfig;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class RemoteConfigService
    {
        public GameConfigs Config { get; private set; }

        [Inject]
        public void Construct()
        {
            LoadConfigs();
        }

        public void LoadConfigs()
        {
            string json = FirebaseRemoteConfig.DefaultInstance.GetValue("AsteroidData").StringValue;
            Debug.Log("Raw JSON from Remote Config: " + json);
            Config = JsonUtility.FromJson<GameConfigs>(json);
            Debug.Log("Configs parsed: " + JsonUtility.ToJson(Config));
        }
    }
}
