using System.Collections;

namespace Asteroids
{
    public interface IFiringScript
    {
        public void Fire();

        public IEnumerator Recharge();
    
    }
}
