using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class SceneService
    {
        public event Action OnExitGame;

        public void ReloadGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        public void ExitGame()
        {
            OnExitGame?.Invoke();

            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
