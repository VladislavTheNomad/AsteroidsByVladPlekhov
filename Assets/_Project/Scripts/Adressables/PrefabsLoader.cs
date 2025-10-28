using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Asteroids
{
    public class PrefabsLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference asteroidPrefabRef;
        [SerializeField] private AssetReference bulletPrefabRef;
        [SerializeField] private AssetReference ufoPrefabRef;

        private DiContainer _container;
        private int _poolSize;

        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

        [Inject]
        public void Construct(DiContainer container, int poolSize)
        {
            _container = container;
            _poolSize = poolSize;
        }

        public async void LoadAndBindToPool()
        {

            AsyncOperationHandle<GameObject> asteroidTask = asteroidPrefabRef.LoadAssetAsync<GameObject>();
            AsyncOperationHandle<GameObject> bulletTask = bulletPrefabRef.LoadAssetAsync<GameObject>();
            AsyncOperationHandle<GameObject> ufoTask = ufoPrefabRef.LoadAssetAsync<GameObject>();

            await Task.WhenAll(asteroidTask.Task, bulletTask.Task, ufoTask.Task);

            if (asteroidTask.Status == AsyncOperationStatus.Succeeded)
            {
                _container.BindMemoryPool<AsteroidView, MonoMemoryPool<AsteroidView>>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(asteroidTask.Result).
                UnderTransformGroup("Asteroids");
            }

            if (bulletTask.Status == AsyncOperationStatus.Succeeded)
            {
                _container.BindMemoryPool<BulletView, BulletPool>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(bulletTask.Result).
                UnderTransformGroup("Bullets");
            }

            if (ufoTask.Status == AsyncOperationStatus.Succeeded)
            {
                _container.BindMemoryPool<UfoView, MonoMemoryPool<UfoView>>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(ufoTask.Result).
                UnderTransformGroup("UFO's");
            }
        }

        private void OnDestroy()
        {
            foreach (var handle in _handles)
            {
                Addressables.Release(handle);
            }
            _handles.Clear();
        }
    }
}
