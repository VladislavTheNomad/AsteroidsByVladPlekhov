using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PoolManager<T> where T : Component
    {
        private GameObject _objectPrefab;
        private int _poolSize;
        private UIManager _uiManager;
        private List<T> _inactiveObjectsPool;
        private List<T> _activeObjectsPool;
        private DiContainer _container;
        private List<Component> _subsribedComponents = new List<Component>();

        public PoolManager(GameObject prefab, int size, DiContainer container)
        {
            _objectPrefab = prefab;
            _poolSize = size;

            _container = container;
            _uiManager = _container.TryResolve<UIManager>();

            _inactiveObjectsPool = new List<T>(_poolSize);
            _activeObjectsPool = new List<T>(_poolSize);

            for (int i = 0; i < _poolSize; i++)
            {
                AddNewObjectInPool();
            }
        }

        public T Get()
        {
            if (_inactiveObjectsPool.Count > 0)
            {
                T obj = _inactiveObjectsPool[0];
                _inactiveObjectsPool.RemoveAt(0);
                _activeObjectsPool.Add(obj);
                return obj;
            }
            else
            {
                AddNewObjectInPool();
                return Get();
            }
        }

        public void Return(T returnedObject)
        {
            returnedObject.gameObject.SetActive(false);
            _activeObjectsPool.Remove(returnedObject);
            _inactiveObjectsPool.Add(returnedObject);
        }

        private void AddNewObjectInPool()
        {
            GameObject newObject = GameObject.Instantiate(_objectPrefab, Vector3.zero, Quaternion.identity);
            newObject.TryGetComponent<T>(out T component);

            _container.InjectGameObject(newObject);
            newObject.SetActive(false);

            if (newObject.TryGetComponent<AsteroidPresenter>(out AsteroidPresenter asteroidScript))
            {
                if (this is PoolManager<AsteroidPresenter>)
                {
                    asteroidScript.Initialize();
                    _uiManager.SubscribeOnDeath(asteroidScript);
                    asteroidScript.OnDeath += HandleRetrnToPool;
                    _subsribedComponents.Add(asteroidScript);
                }
            }

            if (newObject.TryGetComponent<BulletPresenter>(out var bulletScript))
            {
                if (this is PoolManager<BulletPresenter>)
                {
                    bulletScript.Initialize();
                    bulletScript.OnDeath += HandleRetrnToPool;
                    _subsribedComponents.Add(bulletScript);
                }
            }

            if (newObject.TryGetComponent<UfoPresenter>(out var ufoScript))
            {
                if (this is PoolManager<UfoPresenter>)
                {
                    ufoScript.Initialize();
                    _uiManager.SubscribeOnDeath(ufoScript);
                    ufoScript.OnDeath += HandleRetrnToPool;
                    _subsribedComponents.Add(ufoScript);
                }
            }

            _inactiveObjectsPool.Add(component);
        }

        private void HandleRetrnToPool(Component obj)
        {
            Return((T)(object)obj);
        }

        public void ClearPool()
        {
            _inactiveObjectsPool.Clear();
            _activeObjectsPool.Clear();

            foreach (var comp in _subsribedComponents)
            {
                if (comp == null) continue;

                if (comp is AsteroidPresenter asteroidBehComp)
                {
                    asteroidBehComp.OnDeath -= HandleRetrnToPool;
                }
                else if (comp is UfoPresenter ufoBehComp)
                {
                    ufoBehComp.OnDeath -= HandleRetrnToPool;
                }
                else if (comp is BulletPresenter bulletBehComp)
                {
                    bulletBehComp.OnDeath -= HandleRetrnToPool;
                }
            }
        }
    }
}