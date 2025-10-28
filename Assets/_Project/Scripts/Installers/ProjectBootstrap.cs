using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;

namespace Asteroids
{
    public class ProjectBootstrap : MonoBehaviour
    {
        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                DependencyStatus status = task.Result;

                if (status == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                }
                else
                {
                    Debug.LogError("Ошибка зависимостей Firebase: " + status);
                }
            });

            SceneManager.LoadScene("MainScene");
        }
    }
}