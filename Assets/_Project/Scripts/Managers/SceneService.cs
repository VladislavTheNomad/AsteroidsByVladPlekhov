using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class SceneService
    {
        private const string MAIN_SCENE_NAME = "MainScene";
        private const string MENU_GAME_SCENE = "Menu";
        
        public event Action OnExitGame;

        public void StartGame()
        {
            SceneManager.LoadScene(MAIN_SCENE_NAME);
        }
        
        public void OpenMenu()
        {
            SceneManager.LoadScene(MENU_GAME_SCENE);
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
