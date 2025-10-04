using Zenject;

namespace Asteroids
{
    public class BulletPool : MonoMemoryPool<BulletView>
    {
        protected override void OnSpawned(BulletView item)
        {
            item.gameObject.SetActive(false);
        }

        protected override void OnDespawned(BulletView item)
        {
            item.StopBulletMovement();
            item.gameObject.SetActive(false);
        }
    }
}
