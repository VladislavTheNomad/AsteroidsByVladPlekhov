using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class UfoBehaviour : MonoBehaviour, IHaveDeathConditions
    {
        // connections
        [SerializeField] private PlayerController player;
        [SerializeField] private UIManager uiManager;

        //own
        private Vector3 destination;
        private bool isReviveTime;

        //settings
        [SerializeField] private float moveSpeed;
        [SerializeField] private float gapBetweenPositionChanging;
        [SerializeField] private float timeForRevive;

        private void OnEnable()
        {
            StartCoroutine(ChangeTargetPosition());
            Vector2 spawnPlace = UtilsMakeRandomStartPosition.MakeRandomStartPosition();
            transform.position = spawnPlace;
        }

        private IEnumerator ChangeTargetPosition()
        {
            while (true)
            {
                yield return new WaitForSeconds(gapBetweenPositionChanging);
                destination = (player.transform.position - transform.position).normalized;
            }
        }

        private void Update()
        {
            if (!isReviveTime)
            {
                transform.position += destination * moveSpeed * Time.deltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.GetComponent<BulletBehaviour>()) return;

            DeathConditions();
        }

        private IEnumerator Revive()
        {
            yield return new WaitForSeconds(timeForRevive);
            isReviveTime = false;
        }

        public void DeathConditions()
        {
            isReviveTime = true;
            Vector2 spawnPlace = UtilsMakeRandomStartPosition.MakeRandomStartPosition();
            transform.position = spawnPlace;
            StartCoroutine(Revive());
            uiManager.UpdateScore(1);
        }

        public void SetUIManager(UIManager UImanager)
        {
            uiManager = UImanager;
        }
    }
}