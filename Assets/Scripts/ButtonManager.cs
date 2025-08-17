using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class ButtonManager : MonoBehaviour
    {
        public void RetryButtonCLick()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("MainScene");
        }

        public void QuitButtonCLick()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
