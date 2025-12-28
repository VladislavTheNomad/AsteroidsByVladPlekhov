using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Asteroids
{
    public interface IAssetProvider
    {
        UniTask<T> LoadComponent<T>(string address) where T : Component;
        
        UniTask<T> LoadObject<T>(string address) where T : Object;
        
        void Unload();
    }
}
