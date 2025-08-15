using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class LaserScript : MonoBehaviour
    {
        //settings
        [SerializeField] private float laserLifeTime;
        [SerializeField] private float moveSpeed;


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
            yield return new WaitForSeconds(laserLifeTime);
            gameObject.SetActive(false);
        }
    }
}
