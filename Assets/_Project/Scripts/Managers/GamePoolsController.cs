using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GamePoolsController : MonoBehaviour, IInitializable
    {
        [SerializeField] private GameObject _prefabBullet;
        [SerializeField] private GameObject _prefabAsteroid;
        [SerializeField] private GameObject _prefabUFO;
        [SerializeField, Range(1, 100)] private int _poolSize;
        [SerializeField] private GameObject _player;

        private PoolManager<AsteroidView> _asteroidPool;
        private PoolManager<BulletView> _bulletPool;
        private PoolManager<UfoView> _ufoPool;
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void OnDestroy()
        {
            _asteroidPool.ClearPool();
            _bulletPool.ClearPool();
            _ufoPool.ClearPool();
        }

        public void Initialize()
        {
            _asteroidPool = new PoolManager<AsteroidView>(_prefabAsteroid, _poolSize, _container);
            _bulletPool = new PoolManager<BulletView>(_prefabBullet, _poolSize, _container);
            _ufoPool = new PoolManager<UfoView>(_prefabUFO, _poolSize, _container);
        }

        public PoolManager<AsteroidView> GetAsteroidPool() => _asteroidPool;
        public PoolManager<BulletView> GetBulletPool() => _bulletPool;
        public PoolManager<UfoView> GetUFOPool() => _ufoPool;
    }
}