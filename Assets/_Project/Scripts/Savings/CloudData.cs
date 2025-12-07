using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

namespace Asteroids
{

    public class CloudData : IInitializable
    {
        public async void Initialize()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void SaveDataToCloud(int scoreToSave)
        {
            string dateToSave = DateTime.Now.ToString("yyyy:MM:dd - HH:mm:ss");

            var saveData = new Dictionary<string, object>()
            {
                {"BestScore" , scoreToSave},
                {"BestScoreDate", dateToSave},
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
            Debug.Log($"Saved Data: {string.Join(',', saveData)}");
        }


        public async Task<(int score, string date)> LoadDataFromCloud()
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "BestScore", "BestScoreDate" });

            if (playerData.TryGetValue("BestScore", out var value))
            {
                int loadedBestScore = value.Value.GetAs<int>();

                if (playerData.TryGetValue("BestScoreDate", out var valueDate))
                {
                    string dateStr = valueDate.Value.GetAs<string>();

                    return (loadedBestScore, dateStr);
                }

                return (loadedBestScore, "");
            }

            return (0, "");
        }
    }
}
