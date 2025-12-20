using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GlobalAssetCache
    {
        private const string ASTEROID_ADDRESS = "Asteroid";
        private const string BULLET_ADDRESS = "Bullet";
        private const string UFO_ADDRESS = "Ufo";
        
        private GameObject _asteroidPrefab;
        private GameObject _bulletPrefab;
        private GameObject _ufoPrefab;
        
        public bool IsLoaded { get; private set; }
        
        public GameObject GetAsteroidPrefab() { return _asteroidPrefab; }
        public GameObject GetBulletPrefab() { return _bulletPrefab; }
        public GameObject GetUFOPrefab() { return _ufoPrefab; }
        
        private readonly IAssetProvider _assetProvider;

        [Inject]
        public GlobalAssetCache(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask LoadGameAssetsAsync()
        {
            if (IsLoaded) return;
            
            var asteroidView = await _assetProvider.Load<AsteroidView>(ASTEROID_ADDRESS);
            var bulletView = await _assetProvider.Load<BulletView>(BULLET_ADDRESS);
            var ufoView = await _assetProvider.Load<UfoView>(UFO_ADDRESS);
            
            _asteroidPrefab = asteroidView.gameObject;
            _bulletPrefab = bulletView.gameObject;
            _ufoPrefab = ufoView.gameObject;
            
            IsLoaded = true;
            Debug.Log("[GlobalAssetCache] All assets loaded successfully.");
        }
    
    }
}
