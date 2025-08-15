using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class BulletPoolManager : MonoBehaviour, IInitiable
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int numOfBulletInPool;

        private List<GameObject> bulletPool;
        public int sortingIndex => 2;

        public void Installation()
        {
            bulletPool = new List<GameObject>();
            for (int i = 0; i < numOfBulletInPool; i++)
            {
                GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

                var bulletScript = newBullet.GetComponent<BulletBehaviour>();
                bulletScript.SetBulletPoolManager(this);

                newBullet.SetActive(false);
                bulletPool.Add(newBullet);
            }
        }

        public GameObject GetBullet()
        {
            for (int i = 0; i < bulletPool.Count; i++)
            {
                if (!bulletPool[i].activeSelf)
                {
                    return bulletPool[i];
                }
            }
            return null;
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
        }
    }
}
