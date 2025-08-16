using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class LaserVisual : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        //settings
        [SerializeField] private float laserLength = 20f;
        [SerializeField] private float laserDuration = 0.5f;
        [SerializeField] private float laserWidth = 0.001f;

        public void ShowLaserVisual()
        {
            StartCoroutine(LaserRoutine());
        }

        public void SetLineRenderer(LineRenderer lr)
        {
            lineRenderer = lr;
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
        }

        private IEnumerator LaserRoutine()
        {
            lineRenderer.enabled = true;

            Vector3 start = transform.position;
            Vector3 end = start + transform.up * laserLength;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            yield return new WaitForSeconds(laserDuration);

            lineRenderer.enabled = false;
        }
    }
}
