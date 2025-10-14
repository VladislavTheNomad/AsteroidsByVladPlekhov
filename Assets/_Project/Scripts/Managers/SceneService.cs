using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class SceneService
    {
        public event Action GameIsPaused;
        public event Action GameIsUnpaused;

        private bool isPaused;

        public void ReloadGame()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        public void PauseGame()
        {
            if(!isPaused)
            {
                isPaused = true;
                GameIsPaused?.Invoke();
            }
            else
            {
                isPaused = false;
                GameIsUnpaused.Invoke();
            }
        }

        public void ExitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
