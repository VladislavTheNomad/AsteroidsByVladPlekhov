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

        public Vector3 GetNewDestination(Transform transform)
        {
            return (_playerView.transform.position - transform.position).normalized;
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