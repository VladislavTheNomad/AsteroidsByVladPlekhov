using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class FireLaser : MonoBehaviour, IFiringScript
    {
        //settings
        [SerializeField] private int maxLaserShoots;
        [SerializeField] private int currentLaserShoots;
        [SerializeField] private float rechargeTime;

        [SerializeField] private float laserDistance;
        [SerializeField] private LayerMask destructableLayers;

        //connections
        [SerializeField] GameObject leftBound;
        [SerializeField] GameObject rightBound;


        public void Fire()
        {
            if(currentLaserShoots > 0)
            {
                currentLaserShoots--;
                StartCoroutine(Recharge());
                RayCastGo(leftBound);
                RayCastGo(rightBound);
            }
        }

        private void RayCastGo(GameObject bound)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(bound.transform.position, transform.up, laserDistance, destructableLayers);
            foreach (var hit in hits)
            {
                IHaveDeathConditions thisObject = hit.collider.GetComponent<IHaveDeathConditions>();
                thisObject.DeathConditions();
            }
        }

        public IEnumerator Recharge()
        {
            yield return new WaitForSeconds(rechargeTime);
            currentLaserShoots++;
        }
    }
}
