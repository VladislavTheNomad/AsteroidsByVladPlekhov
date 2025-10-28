using System;

namespace Asteroids
{
    public class PauseGame
    {
        public event Action<bool> GameIsPaused;

        private bool isPaused;

        public void PauseGameProcess()
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
