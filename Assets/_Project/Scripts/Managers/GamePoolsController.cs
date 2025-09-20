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

        private PoolManager<AsteroidPresenter> _asteroidPool;
        private PoolManager<BulletPresenter> _bulletPool;
        private PoolManager<UfoPresenter> _ufoPool;
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
            _asteroidPool = new PoolManager<AsteroidPresenter>(_prefabAsteroid, _poolSize, _container);
            _bulletPool = new PoolManager<BulletPresenter>(_prefabBullet, _poolSize, _container);
            _ufoPool = new PoolManager<UfoPresenter>(_prefabUFO, _poolSize, _container);
        }

        public PoolManager<AsteroidPresenter> GetAsteroidPool() => _asteroidPool;
        public PoolManager<BulletPresenter> GetBulletPool() => _bulletPool;
        public PoolManager<UfoPresenter> GetUFOPool() => _ufoPool;
    }
}