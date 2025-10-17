using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class SceneService
    {
        public void ReloadGame()
        {
            SceneManager.LoadScene("MainScene");
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
