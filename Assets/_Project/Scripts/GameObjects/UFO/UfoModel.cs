using UnityEngine;

namespace Asteroids
{
    public class UfoModel
    {
        public int ScorePoints { get; private set; }
        public int MoveSpeed { get; private set; }
        public int GapBetweenPositionChanging { get; private set; }

        private PlayerView _playerView;

        public UfoModel(UFOConfig config, PlayerView playerView) 
        {
            _playerView = playerView;

            ScorePoints = config.ScorePoints;
            MoveSpeed = config.MoveSpeed;
            GapBetweenPositionChanging = config.GapBetweenPositionChanging;
        }

        public void GetNewDestination(Transform transform, out Vector3 destination, out float moveSpeed)
        {
            destination = (_playerView.transform.position - transform.position).normalized;
            moveSpeed = MoveSpeed;
        }

        public bool CheckNull(Transform transform)
        {
            if (transform == null || _playerView == null || !_playerView.gameObject.activeSelf)
            {
                return false;
            }
            return true;
        }
    }
}