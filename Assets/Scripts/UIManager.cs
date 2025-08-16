using UnityEngine;

namespace Asteroids
{
    public class UIManager : MonoBehaviour, IInitiable
    {
        public int sortingIndex => 999;

        //connections
        [SerializeField] private DeathConditions playerDeadConditions;

        public void Installation()
        {
            playerDeadConditions.OnPlayerIsDead += PlayerIsDead;
        }

        private void PlayerIsDead()
        {
            Debug.Log("1");
        }
    }
}
