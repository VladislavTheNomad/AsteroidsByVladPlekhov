using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class FireBullet : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 5f)] private float _rechargeTime = 0.5f;

        private bool _canFire;
        private GamePoolsController _gamePoolsController;

        [Inject]
        public void Constuct(GamePoolsController gpc)
        {
            _gamePoolsController = gpc;
        }

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