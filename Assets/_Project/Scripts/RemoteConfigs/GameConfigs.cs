using System;
using UnityEngine;

namespace Asteroids
{
    [Serializable]
    public class GameConfigs
    {
        // asteroids
        public float MaxMoveSpeed = 60f;
        public float MinMoveSpeed = 30f;
        public int AccelerationModificator = 2;
        public int MaxSizeLevel = 3;
        public int SmallAsteroidQuantity = 3;
        public int AsteroidScorePoints = 1;
        public float ScaleReduce = 0.5f;

        //bullet
        public float BulletsLifeTime = 2f;
        public float BulletMoveSpeed = 10f;

        //player
        public int MaxLaserShots = 3;
        public float LaserDistance = 55f;
        public float RotationSpeed = 40f;
        public float MovementSpeed = 10f;
        public float BulletRechargeTime = 0.4f;
        public float LaserRechargeTime = 10f;
        public LayerMask DestructableLayers = LayerMask.GetMask("Enemy");
        public float InvincibleTime = 3f;

        //UFO
        public int UFOScorePoints = 3;
        public float UFOMoveSpeed = 3f;
        public float GapBetweenPositionChanging = 1f;

        //GameProcess
        public float TimeBetweenAsteroidsSpawns = 1f;
        public float TimeBetweenUFOSpawns = 5f;
    }
}
