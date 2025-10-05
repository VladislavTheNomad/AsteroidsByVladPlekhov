using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class UtilsCalculatePositions : IInitializable
    {
        private const float HEIGHT_BUFFER = 0.2f;
        private const float WIDTH_BUFFER = 0.3f;
        private const float SCREEN_RIGHT_BOUND = 1f;
        private const float SCREEN_LEFT_BOUND = 0f;
        private const float SCREEN_TOP_BOUND = 1f;
        private const float SCREEN_BOTTOM_BOUND = 0f;
        private const int NUMBER_OF_SCREEN_SIDES = 4;
        private const int SPAWN_OFFSET = 10;

        private Camera _camera;
        private Vector2 _bottomLeft;
        private Vector2 _topRight;
        private float _widthScreen;
        private float _heightScreen;

        public UtilsCalculatePositions(Camera camera)
        {
            _camera = camera;
        }

        public void Initialize()
        {
            _bottomLeft = _camera.ViewportToWorldPoint(Vector2.zero);
            _topRight = _camera.ViewportToWorldPoint(Vector2.one);
            _widthScreen = _topRight.x - _bottomLeft.x;
            _heightScreen = _topRight.y - _bottomLeft.y;
        }

        public Vector3 CheckBounds(Transform transform)
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(transform.position);
            bool isOutOfBounds =
        viewportPos.x < SCREEN_LEFT_BOUND - HEIGHT_BUFFER ||
        viewportPos.x > SCREEN_RIGHT_BOUND + HEIGHT_BUFFER ||
        viewportPos.y > SCREEN_TOP_BOUND + WIDTH_BUFFER ||
        viewportPos.y < SCREEN_BOTTOM_BOUND - WIDTH_BUFFER;


            if (isOutOfBounds)
            {
                Vector3 newPosition = transform.position;

                if (viewportPos.x > SCREEN_RIGHT_BOUND)
                {
                    newPosition.x = _bottomLeft.x;
                }
                else if (viewportPos.x < SCREEN_LEFT_BOUND)
                {
                    newPosition.x = _topRight.x;
                }

                if (viewportPos.y > SCREEN_TOP_BOUND)
                {
                    newPosition.y = _bottomLeft.y;
                }
                else if (viewportPos.y < SCREEN_BOTTOM_BOUND)
                {
                    newPosition.y = _topRight.y;
                }
                return newPosition;
            }
            return Vector3.zero;
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