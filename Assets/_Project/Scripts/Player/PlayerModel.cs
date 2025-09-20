using UnityEngine;

namespace Asteroids
{
    public class PlayerModel
    {
        private const int MAX_LASER_SHOTS = 3;

        public Vector3 Position { get; private set; }
        public float Rotation { get; private set; }
        public float CurrentSpeed { get; private set; }
        public float MovementSpeed { get; private set; }
        public float RotationSpeed { get; private set; }
        public int LaserShots { get; private set; }
        public float BulletRechargeTime { get; private set; }
        public float LaserRechargeTime { get; private set; }
        public float[] LaserRechargeTimers { get; private set; }
        public bool CanFireBullet { get; private set; }
        public LayerMask DestructableLayers { get; private set; }

        public PlayerModel(PlayerConfig playerConfig)
        {
            Position = Vector3.zero;
            Rotation = 0f;
            CurrentSpeed = 0f;
            LaserShots = MAX_LASER_SHOTS;
            CanFireBullet = true;
            LaserRechargeTimers = new float[MAX_LASER_SHOTS];
            for (int i = 0; i < LaserRechargeTimers.Length; i++)
            {
                LaserRechargeTimers[i] = 0f;
            }

            MovementSpeed = playerConfig.MovementSpeed;
            RotationSpeed = playerConfig.RotationSpeed;
            BulletRechargeTime = playerConfig.BulletRechargeTime;
            LaserRechargeTime = playerConfig.LaserRechargeTime;
            DestructableLayers = playerConfig.DestructableLayers;
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public void UpdateRotation(float newRotation)
        {
            Rotation = newRotation;
        }

        public void UpdateSpeed(float newSpeed)
        {
            CurrentSpeed = newSpeed;
        }

        public void IncreaseLaserShots()
        {
            if (LaserShots < MAX_LASER_SHOTS)
            {
                LaserShots++;
            }
        }

        public void DecreaseLaserShots()
        {
            if (LaserShots > 0)
            {
                LaserShots--;
            }
        }

        public void SetCanFireBullet(bool canFire)
        {
            CanFireBullet = canFire;
        }

        public void SetLazerRechargeTimer(int index, float value)
        {
            LaserRechargeTimers[index] = value;
        }

        public void SubtractLazerRechargeTimer(int index, float value)
        {
            LaserRechargeTimers[index] -= value;
        }

        public int GetMaxLaserShots() => MAX_LASER_SHOTS;
    }
}