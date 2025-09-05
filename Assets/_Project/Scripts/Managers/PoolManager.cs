using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class PoolManager<T> where T : Component
    {
        private GameObject _objectPrefab;
        private int _poolSize;
        private UIManager _uiManager;
        private List<T> _inactiveObjectsPool;
        private List<T> _activeObjectsPool;
        private Camera _camera;
        private GameObject _playerObject;
        private UtilsCalculatePositions _calculatePositions;

        public void Installation(GameObject prefab, int size, UIManager manager, Camera camera, GameObject player, UtilsCalculatePositions _utilsCalculatePositions)
        {
            _objectPrefab = prefab;
            _poolSize = size;
            _uiManager = manager;
            _camera = camera;
            _playerObject = player;
            _calculatePositions = _utilsCalculatePositions;

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

        public Camera GetCamera()
        {
            return _camera;
        }

        private void AddNewObjectInPool()
        {
            GameObject newObject = GameObject.Instantiate(_objectPrefab, Vector3.zero, Quaternion.identity);
            newObject.SetActive(false);
            T component = newObject.GetComponent<T>();

            if (newObject.TryGetComponent<AsteroidBehaviour>(out AsteroidBehaviour asteroidScript))
            {
                if (this is PoolManager<AsteroidBehaviour> asteroidPool)
                {
                    asteroidScript.Initialize(asteroidPool, _calculatePositions);
                    _uiManager.SubscribeOnDeath(asteroidScript);
                    asteroidScript.OnDeathReturnToPool += obj => Return((T)(object)obj);
                }
            }

            if (newObject.TryGetComponent<BulletBehaviour>(out var bulletScript))
            {
                if (this is PoolManager<BulletBehaviour> bulletPool)
                {
                    bulletScript.Initialize();
                    bulletScript.OnDeathReturnToPool += obj => Return((T)(object)obj);
                }
            }

            if (newObject.TryGetComponent<UfoBehaviour>(out var ufoScript))
            {
                if (this is PoolManager<UfoBehaviour> ufoPool)
                {
                    ufoScript.Initialize(_playerObject);
                    _uiManager.SubscribeOnDeath(ufoScript);
                    ufoScript.OnDeathReturnToPool += obj => Return((T)(object)obj);
                }
            }

            _inactiveObjectsPool.Add(component);
        }
    }
}