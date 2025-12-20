using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class BootstrapTrigger : MonoBehaviour
    {
        private ProjectBootstrap _bootstrap;

        [Inject]
        public void Construct(ProjectBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
        }

        private void Start()
        {
            Debug.Log("Bootstrap started...");
            //_bootstrap.Initialize();
        }
    
    }
}
