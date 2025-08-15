using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class BulletBehaviour : MonoBehaviour
    {
        private BulletPoolManager bulletPoolManager;

        //settings
        [SerializeField] private float bulletsLifeTime;
        [SerializeField] private float moveSpeed;

        public void SetBulletPoolManager(BulletPoolManager manager)
        {
            bulletPoolManager = manager;
        }

        private void OnEnable()
        {
            StartCoroutine(LifeTime());
        }

        private void Update()
        {
            transform.position += transform.up * moveSpeed * Time.deltaTime;
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(bulletsLifeTime);
            bulletPoolManager.ReturnBullet(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.GetComponent<AsteroidBehaviour>()) return;

            bulletPoolManager.ReturnBullet(gameObject);
        }
    }
}
