namespace Asteroids
{
    public class BulletModel
    {
        public float BulletsLifeTime { get; private set; }
        public float MoveSpeed { get; private set; }

        public BulletModel(float bulletsLifeTime, float moveSpeed)
        {
            BulletsLifeTime = bulletsLifeTime;
            MoveSpeed = moveSpeed;
        }
    }
}