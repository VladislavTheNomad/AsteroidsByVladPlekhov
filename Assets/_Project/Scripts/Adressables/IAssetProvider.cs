using UnityEngine;

namespace Asteroids
{
    public interface IAssetProvider
    {
        T Load<T>(string address) where T : Component;
        void Unload();
    }
}
