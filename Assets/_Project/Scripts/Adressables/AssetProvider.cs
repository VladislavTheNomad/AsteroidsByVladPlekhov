using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Asteroids
{
    public class AssetProvider : IAssetProvider, IDisposable
    {
        private List<AsyncOperationHandle<GameObject>> _handles = new List<AsyncOperationHandle<GameObject>>();

        public void Dispose()
        {
            Unload();
        }

        public T Load<T>(string address) where T : Component
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            _handles.Add(handle);


            GameObject prefab = handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                T component = prefab.GetComponent<T>();

                if(component != null)
                {
                    return component;
                }
                else
                {
                    Debug.LogError($"Component: {typeof(T)} not found!");
                }
            }
            else
            {
                Debug.LogError($"[AddressablesAssetProvider] Failed to load asset at address: {address}");
            }

            return null;
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
