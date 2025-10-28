using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Asteroids
{
    public class AssetProvider : IAssetProvider
    {
        public GameObject LoadPrefab(string address)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            GameObject prefab = handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return prefab;
            }
            else
            {
                Debug.LogError($"[AddressablesAssetProvider] Failed to load asset at address: {address}");
            }
            
            return null;
        }
    }
}
