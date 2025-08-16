using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class PlayerController : MonoBehaviour, IInitiable
    {
        //connections
        [SerializeField] private FireBullet fireBulletScript;
        [SerializeField] private FireLaser fireLaserScript;
        [SerializeField] private LaserVisual laserVisual;


        private PlayerControls playerControls;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Camera mainCamera;

        private float moveInput;
        private float rotateInput;
        private float shootBulletInput;

        //settings
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float movementSpeed;


        public int sortingIndex { get; private set; } = 1;

        public void Installation()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = FindAnyObjectByType<Camera>();

            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            laserVisual.SetLineRenderer(lineRenderer);

            playerControls = new PlayerControls();
            playerControls.Player.Enable();
            playerControls.Player.Move.performed += context => moveInput = context.ReadValue<float>();
            playerControls.Player.Move.canceled += context => moveInput = 0f;
            playerControls.Player.Rotate.performed += context => rotateInput = context.ReadValue<float>();
            playerControls.Player.Rotate.canceled += context => rotateInput = 0f;

            playerControls.Player.ShootBullet.performed += context => shootBulletInput = 1f;
            playerControls.Player.ShootBullet.canceled += context => shootBulletInput = 0f;

            playerControls.Player.ShootLaser.started += context => ShootLaser();

        }

        private void ShootLaser()
        {
            fireLaserScript.Fire();
            laserVisual.ShowLaserVisual();
        }

        private void Update()
        {
            //shooting
            if (shootBulletInput > 0f)
            {
                fireBulletScript.Fire();
            }
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
    }
}
