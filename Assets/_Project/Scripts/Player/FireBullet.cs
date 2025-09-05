using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class FireBullet : MonoBehaviour
    {
        [SerializeField] private GamePoolsController _gamePoolsController;
        [SerializeField, Range(0.1f, 5f)] private float _rechargeTime = 0.5f;

        private bool _canFire;

        public void ShootBullet()
        {
            if (!_canFire)
            {
                StartCoroutine(Recharge());
                BulletBehaviour bulletSpawn = _gamePoolsController.GetBulletPool().Get();
                bulletSpawn.transform.SetPositionAndRotation(transform.position, transform.rotation);
                bulletSpawn.gameObject.SetActive(true);
            }
        }

        public IEnumerator Recharge()
        {
            _canFire = true;
            yield return new WaitForSeconds(_rechargeTime);
            _canFire = false;
        }
    }
}