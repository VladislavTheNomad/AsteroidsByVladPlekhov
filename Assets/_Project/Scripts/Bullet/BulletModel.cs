using System.Threading.Tasks;

namespace Asteroids
{
    public class BulletModel
    {
        public float BulletsLifeTime { get; private set; }
        public float MoveSpeed { get; private set; }

        public BulletModel(BulletConfig bulletConfig)
        {
            BulletsLifeTime = bulletConfig.BulletsLifeTime;
            MoveSpeed = bulletConfig.MoveSpeed;
        }
    }
}