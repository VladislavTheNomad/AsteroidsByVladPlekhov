using System;

namespace Asteroids
{
    public class PauseManager
    {
        public event Action<bool> GameIsPaused;

        private bool isPaused;

        public void PauseGame()
        {
            if (!isPaused)
            {
                isPaused = true;
                GameIsPaused?.Invoke(true);
            }
            else
            {
                isPaused = false;
                GameIsPaused?.Invoke(false);
            }
        }
    }
}
