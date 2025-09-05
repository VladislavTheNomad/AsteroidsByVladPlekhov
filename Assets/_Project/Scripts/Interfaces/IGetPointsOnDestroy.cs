using System;

namespace Asteroids
{
    public interface IGetPointsOnDestroy
    {
        public event Action<int> OnDeathTakeScore;
    }
}