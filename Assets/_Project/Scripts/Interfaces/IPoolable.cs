using System;
using UnityEngine;

namespace Asteroids
{
    public interface IPoolable<T> where T : Component
    {
        public event Action<T> OnDeathReturnToPool;
    }
}
