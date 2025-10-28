using UnityEngine;

namespace Asteroids
{
    public interface IAssetProvider
    {
        GameObject LoadPrefab(string address);
    }
}
