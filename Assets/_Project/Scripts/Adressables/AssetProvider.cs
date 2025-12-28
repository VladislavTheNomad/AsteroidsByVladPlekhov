using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Asteroids
{
    public class AssetProvider : IAssetProvider, IDisposable
    {
        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

        public void Dispose()
        {
            Unload();
        }

        public async UniTask<T> LoadComponent<T>(string address) where T : Component
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            _handles.Add(handle);

            try
            {
                GameObject prefab = await handle.ToUniTask();
                
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (prefab.TryGetComponent(out T component))
                    {
                        return component;
                    }
                    
                    Debug.LogError($"Component: {typeof(T)} not found on prefab at {address}!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading asset: {ex.Message}");
            }
            
            return null;
        }
        
        public async UniTask<T> LoadObject<T>(string address) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            _handles.Add(handle);

            try
            {
                return await handle.ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading raw asset {address}: {ex.Message}");
                return null;
            }
            
        }

        public void Unload()
        {
            foreach (var handle in _handles)
            {
                if(handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _handles.Clear();
        }
    }
}
