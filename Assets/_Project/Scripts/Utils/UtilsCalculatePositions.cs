using UnityEngine;

namespace Asteroids
{
    public class UtilsCalculatePositions : MonoBehaviour, IInitiable
    {
        private const int NUMBER_OF_SCREEN_SIDES = 4;
        private const int SPAWN_OFFSET = 10;

        private Camera _camera;
        private Vector2 _bottomLeft;
        private Vector2 _topRight;
        private float _widthScreen;
        private float _heightScreen;

        public void Installation()
        {
            _camera = Camera.main;
            _bottomLeft = _camera.ViewportToWorldPoint(Vector2.zero);
            _topRight = _camera.ViewportToWorldPoint(Vector2.one);
            _widthScreen = _topRight.x - _bottomLeft.x;
            _heightScreen = _topRight.y - _bottomLeft.y;
        }

        public Vector2 GetRandomSpawnPosition()
        {
            int screenSide = Random.Range(0, NUMBER_OF_SCREEN_SIDES);
            float randomPos = Random.value;

            Vector2 spawnRandomPlace = Vector2.zero;

            switch (screenSide)
            {
                case 0:
                    spawnRandomPlace = new Vector2(_bottomLeft.x, _bottomLeft.y + (_heightScreen * randomPos)) + new Vector2(-_heightScreen / SPAWN_OFFSET, 0);
                    break;
                case 1:
                    spawnRandomPlace = new Vector2(_bottomLeft.x + (_heightScreen * randomPos), _bottomLeft.y) + new Vector2(0, -_widthScreen / SPAWN_OFFSET);
                    break;
                case 2:
                    spawnRandomPlace = new Vector2(_topRight.x - (_widthScreen * randomPos), _topRight.y) + new Vector2(0, _widthScreen / SPAWN_OFFSET);
                    break;
                case 3:
                    spawnRandomPlace = new Vector2(_topRight.x, _topRight.y - (_heightScreen * randomPos)) + new Vector2(_heightScreen / SPAWN_OFFSET, 0);
                    break;
            }
            return spawnRandomPlace;
        }
    }
}