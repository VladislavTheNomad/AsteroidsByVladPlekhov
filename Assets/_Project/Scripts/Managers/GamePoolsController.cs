using UnityEngine;

namespace Asteroids
{
    public class GamePoolsController : MonoBehaviour, IInitiable
    {
        [SerializeField] private GameObject _prefabBullet;
        [SerializeField] private GameObject _prefabAsteroid;
        [SerializeField] private GameObject _prefabUFO;
        [SerializeField, Range(1, 100)] private int _poolSize;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _player;
        [SerializeField] private UtilsCalculatePositions _utilsCalculatePositions;

        private PoolManager<AsteroidBehaviour> _asteroidPool;
        private PoolManager<BulletBehaviour> _bulletPool;
        private PoolManager<UfoBehaviour> _ufoPool;

        public void Installation()
        {
            _asteroidPool = new PoolManager<AsteroidBehaviour>();
            _bulletPool = new PoolManager<BulletBehaviour>();
            _ufoPool = new PoolManager<UfoBehaviour>();

            InitializePool(_asteroidPool, _prefabAsteroid);
            InitializePool(_bulletPool, _prefabBullet);
            InitializePool(_ufoPool, _prefabUFO);
        }

        public PoolManager<AsteroidBehaviour> GetAsteroidPool() => _asteroidPool;
        public PoolManager<BulletBehaviour> GetBulletPool() => _bulletPool;
        public PoolManager<UfoBehaviour> GetUFOPool() => _ufoPool;

        private void InitializePool<T>(PoolManager<T> pool, GameObject prefab) where T : Component
        {
            pool.Installation(prefab, _poolSize, _uiManager, _camera, _player, _utilsCalculatePositions);
        }
    }
}