using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class FireBullet : MonoBehaviour, IFiringScript
    {
        //connections
        [SerializeField] private BulletPoolManager bulletPoolManager;

        //own
        private bool isBulletRecharge;

        //settings
        [SerializeField] private float rechargeTime;

        public void Fire()
        {
            if (!isBulletRecharge)
            {
                StartCoroutine(Recharge());
                GameObject bulletSpawn = bulletPoolManager.GetBullet();
                if (bulletSpawn != null)
                {
                    bulletSpawn.transform.position = transform.position;
                    bulletSpawn.transform.rotation = transform.rotation;
                    bulletSpawn.SetActive(true);
                }
            }
        }

        public IEnumerator Recharge()
        {
            isBulletRecharge = true;
            yield return new WaitForSeconds(rechargeTime);
            isBulletRecharge = false;
        }

    }
}
