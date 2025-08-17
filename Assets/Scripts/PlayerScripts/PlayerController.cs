using UnityEngine;

namespace Asteroids
{
    public class PlayerController : MonoBehaviour, IInitiable
    {
        //connections
        [SerializeField] private FireBullet fireBulletScript;
        [SerializeField] private FireLaser fireLaserScript;
        [SerializeField] private LaserVisual laserVisual;
        [SerializeField] private UIManager UIManager;
        [SerializeField] private Camera mainCamera;

        //own
        private PlayerControls playerControls;
        private Rigidbody2D rb;
        private LineRenderer lineRenderer;

        private float moveInput;
        private float rotateInput;
        private float shootBulletInput;
        public int sortingIndex { get; private set; } = 1;

        //settings
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float movementSpeed;

        public void Installation()
        {
            rb = GetComponent<Rigidbody2D>();
            lineRenderer = GetComponent<LineRenderer>();
            laserVisual.SetLineRenderer(lineRenderer);

            fireLaserScript.SetStartAmountOfShots();
            fireLaserScript.SetAmountOfRechareTimers();

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
                UIManager.UpdateSpeed(rb.linearVelocity.magnitude);
            }
        }

        private void LateUpdate()
        {
            //coordinates + rotation angle
            Vector3 playerPositionInCameraCoordinates = mainCamera.WorldToViewportPoint(transform.position);
            UIManager.UpdateCoordinates(playerPositionInCameraCoordinates, transform.eulerAngles.z);
            if (playerPositionInCameraCoordinates.x < 0 || playerPositionInCameraCoordinates.x > 1 || playerPositionInCameraCoordinates.y > 0 || playerPositionInCameraCoordinates.y < 0)
            {
                Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
                Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
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
