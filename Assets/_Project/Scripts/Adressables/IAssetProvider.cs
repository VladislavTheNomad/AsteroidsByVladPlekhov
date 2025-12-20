using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Asteroids
{
    public interface IAssetProvider
    {
        UniTask<T> Load<T>(string address) where T : Component;
        void Unload();
    }
}
