using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class LaserVisual : MonoBehaviour
    {
        private const float LASER_WIDTH = 0.2f;
        private const float LASER_DURATION = 0.25f;
        private const float LASER_LENGTH = 20f;

        private LineRenderer _lineRenderer;

        public void ShowLaserVisual()
        {
            if (_lineRenderer == null) return;
            StartCoroutine(LaserRoutine());
        }

        public void SetLineRenderer(LineRenderer lr)
        {
            _lineRenderer = lr;
            _lineRenderer.enabled = false;
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = LASER_WIDTH;
            _lineRenderer.endWidth = LASER_WIDTH;
        }

        private IEnumerator LaserRoutine()
        {
            if (_lineRenderer == null) yield break;

            _lineRenderer.enabled = true;

            Vector3 start = transform.position;
            Vector3 end = start + transform.up * LASER_LENGTH;

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);

            yield return new WaitForSeconds(LASER_DURATION);

            _lineRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}