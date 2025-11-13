namespace Asteroids
{
    public class BulletModel
    {
        public float BulletsLifeTime { get; private set; }
        public float MoveSpeed { get; private set; }

        public BulletModel(RemoteConfigService configService)
        {
            BulletsLifeTime = configService.Config.BulletsLifeTime;
            MoveSpeed = configService.Config.BulletMoveSpeed;
        }
    }
}