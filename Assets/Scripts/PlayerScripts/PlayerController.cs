using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class PlayerController : MonoBehaviour, IInitiable
    {
        //connections
        [SerializeField] private BulletPoolManager bulletPoolManager;
        [SerializeField] private GameObject laser;

        private PlayerControls playerControls;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Camera mainCamera;
        private bool isBulletRecharge;
        private bool isLaserRecharge;

        private float moveInput;
        private float rotateInput;
        private float shootBulletInput;
        private float shootLaserInput;

        //settings
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rechargeTime;
        [SerializeField] private float laserRechargeTime;
        [SerializeField] private int numberOfLaserCharges;

        private Coroutine laserCharging;

        public int sortingIndex { get; private set; } = 1;

        public void Installation()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = FindAnyObjectByType<Camera>();

            playerControls = new PlayerControls();
            playerControls.Player.Enable();
            playerControls.Player.Move.performed += context => moveInput = context.ReadValue<float>();
            playerControls.Player.Move.canceled += context => moveInput = 0f;
            playerControls.Player.Rotate.performed += context => rotateInput = context.ReadValue<float>();
            playerControls.Player.Rotate.canceled += context => rotateInput = 0f;

            playerControls.Player.ShootBullet.performed += context => shootBulletInput = 1f;
            playerControls.Player.ShootBullet.canceled += context => shootBulletInput = 0f;

            playerControls.Player.ShootLaser.started += context => ShootLaser(); //realize in the method raucast and LineRenderer
            playerControls.Player.ShootLaser.canceled += context => shootLaserInput = 0f;

        }

        private void ShootLaser()
        {
        }

        private void Update()
        {
            //shooting
            if (shootBulletInput > 0f && !isBulletRecharge)
            {
                StartCoroutine(Recharging());
                GameObject bulletSpawn = bulletPoolManager.GetBullet();
                if (bulletSpawn != null)
                {
                    bulletSpawn.transform.position = transform.position;
                    bulletSpawn.transform.rotation = transform.rotation;
                    bulletSpawn.SetActive(true);
                }
            }
        }

        private IEnumerator Recharging()
        {
            isBulletRecharge = true;
            yield return new WaitForSeconds(rechargeTime);
            isBulletRecharge = false;
        }

        private IEnumerator LaserCharging()
        {
            isLaserRecharge = true;
            yield return new WaitForSeconds(laserRechargeTime);
            isLaserRecharge = false;
        }

        private void FixedUpdate()
        {
            // moving
            transform.Rotate(0f, 0f, rotateInput * rotationSpeed * Time.fixedDeltaTime);
            if (moveInput > 0f)
            {
                rb.AddForce(transform.up * movementSpeed, ForceMode2D.Force);
            }

        }

        private void LateUpdate()
        {
            if (!spriteRenderer.isVisible)
            {
                Vector3 playerPositionInCameraCoordinates = mainCamera.WorldToViewportPoint(transform.position);
                Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
                Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));

                if (playerPositionInCameraCoordinates.x < 0 || playerPositionInCameraCoordinates.x > 1 || playerPositionInCameraCoordinates.y > 0 || playerPositionInCameraCoordinates.y < 0)
                {
                    Vector3 newPosition = transform.position;

                    if (playerPositionInCameraCoordinates.x > 1)
                    {
                        newPosition.x = bottomLeft.x;
                    }
                    else if (playerPositionInCameraCoordinates.x < 0)
                    {
                        newPosition.x = topRight.x;
                    }

                    if (playerPositionInCameraCoordinates.y > 1)
                    {
                        newPosition.y = bottomLeft.y;
                    }
                    else if (playerPositionInCameraCoordinates.y < 0)
                    {
                        newPosition.y = topRight.y;
                    }

                    transform.position = newPosition;
                }
            }
        }


        private void OnDeath()
        {
            playerControls.Player.Disable();
        }
    }
}
