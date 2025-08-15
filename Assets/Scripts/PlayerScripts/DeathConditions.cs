using UnityEngine;

namespace Asteroids
{
    public class DeathConditions : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<AsteroidBehaviour>() || collision.GetComponent<UfoBehaviour>())
            {
                Debug.Log("GameOver UI");
                Time.timeScale = 0f;
            }
        }
    }
}
