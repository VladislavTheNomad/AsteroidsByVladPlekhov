using UnityEngine;

namespace Asteroids
{
    public static class UtilsMakeRandomStartPosition
    {

        public static Vector2 MakeRandomStartPosition()
        {
            Camera mainCamera = Camera.main;
            Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(Vector2.zero);
            Vector2 topRight = mainCamera.ViewportToWorldPoint(Vector2.one);
            float widthScreen = topRight.x - bottomLeft.x;
            float heightScreen = topRight.y - bottomLeft.y;

            int dice = Random.Range(0, 4);
            float randomPos = Random.value;

            Vector2 spawnRandomPlace = Vector2.zero;

            switch (dice)
            {
                case 0:  //left side
                    spawnRandomPlace = new Vector2(bottomLeft.x, bottomLeft.y + (heightScreen * randomPos)) + new Vector2(-heightScreen / 10, 0);
                    break;
                case 1: // bottom side
                    spawnRandomPlace = new Vector2(bottomLeft.x + (widthScreen * randomPos), bottomLeft.y) + new Vector2(0, -widthScreen / 10);
                    break;
                case 2: // top side
                    spawnRandomPlace = new Vector2(topRight.x - (widthScreen * randomPos), topRight.y) + new Vector2(0, widthScreen / 10);
                    break;
                case 3: // right side
                    spawnRandomPlace = new Vector2(topRight.x, topRight.y - (heightScreen * randomPos)) + new Vector2(heightScreen / 10, 0);
                    break;
            }
            return spawnRandomPlace;
        }
    }
}
