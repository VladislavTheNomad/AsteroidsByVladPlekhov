using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Asteroids
{
    public class BootstrapManager : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene("MainScene"); 
        }
    }
}