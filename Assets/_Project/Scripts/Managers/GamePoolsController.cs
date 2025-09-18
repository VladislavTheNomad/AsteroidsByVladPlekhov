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

        private PoolManager<AsteroidBehaviour> _asteroidPool;
        private PoolManager<BulletPresenter> _bulletPool;
        private PoolManager<UfoBehaviour> _ufoPool;
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void OnDestroy()
        {
            _asteroidPool.ClearPool();
        }

        public void Initialize()
        {
            _asteroidPool = new PoolManager<AsteroidBehaviour>(_prefabAsteroid, _poolSize, _player, _container);
            _bulletPool = new PoolManager<BulletPresenter>(_prefabBullet, _poolSize, _player, _container);
            _ufoPool = new PoolManager<UfoBehaviour>(_prefabUFO, _poolSize, _player, _container);
        }

        public PoolManager<AsteroidBehaviour> GetAsteroidPool() => _asteroidPool;
        public PoolManager<BulletPresenter> GetBulletPool() => _bulletPool;
        public PoolManager<UfoBehaviour> GetUFOPool() => _ufoPool;
    }
}