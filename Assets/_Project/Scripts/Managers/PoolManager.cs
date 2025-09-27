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
        private List<object> _subsribedComponents = new List<object>();

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

            if (newObject.TryGetComponent<AsteroidView>(out AsteroidView asteroidScript))
            {
                if (this is PoolManager<AsteroidView>)
                {
                    AsteroidPresenter presenter = _container.Instantiate<AsteroidPresenter>();
                    presenter.Initialize(asteroidScript);
                    _uiManager.SubscribeOnDeath(presenter);
                    presenter.OnDeath += HandleRetrnToPool;
                    _subsribedComponents.Add(presenter);
                }
            }

            if (newObject.TryGetComponent<BulletView>(out BulletView bulletScript))
            {
                if (this is PoolManager<BulletView>)
                {
                    BulletPresenter presenter = _container.Instantiate<BulletPresenter>();
                    presenter.Initialize(bulletScript);
                    presenter.OnDeath += HandleRetrnToPool;
                    _subsribedComponents.Add(bulletScript);
                }
            }

            if (newObject.TryGetComponent<UfoView>(out UfoView ufoScript))
            {
                if (this is PoolManager<UfoView>)
                {
                    UfoPresenter presenter = _container.Instantiate<UfoPresenter>();
                    presenter.Initialize(ufoScript);
                    _uiManager.SubscribeOnDeath(presenter);
                    presenter.OnDeath += HandleRetrnToPool;
                    _subsribedComponents.Add(presenter);
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