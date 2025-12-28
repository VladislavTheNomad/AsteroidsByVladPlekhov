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
        private const string MUSIC_ADDRESS = "Music";
        private const string BULLET_SOUND_ADDRESS = "BulletShot";
        private const string LASER_SOUND_ADDRESS = "LaserShot";
        
        
        private GameObject _asteroidPrefab;
        private GameObject _bulletPrefab;
        private GameObject _ufoPrefab;
        
        private AudioClip _bulletSound;
        private AudioClip _laserSound;
        private AudioClip _music;
        
        public bool IsLoaded { get; private set; }
        
        public GameObject GetAsteroidPrefab() => _asteroidPrefab;
        public GameObject GetBulletPrefab() => _bulletPrefab;
        public GameObject GetUFOPrefab() => _ufoPrefab;
        
        public AudioClip GetMusic() => _music;
        public AudioClip GetBulletSound() => _bulletSound;
        public AudioClip GetLaserSound() => _laserSound;
        
        private readonly IAssetProvider _assetProvider;

        [Inject]
        public GlobalAssetCache(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask LoadGameAssetsAsync()
        {
            if (IsLoaded) return;
            
            var asteroidTask = _assetProvider.LoadComponent<AsteroidView>(ASTEROID_ADDRESS);
            var bulletTask = _assetProvider.LoadComponent<BulletView>(BULLET_ADDRESS);
            var ufoTask = _assetProvider.LoadComponent<UfoView>(UFO_ADDRESS);
            
            var musicTask = _assetProvider.LoadObject<AudioClip>(MUSIC_ADDRESS);
            var bulletSoundTask = _assetProvider.LoadObject<AudioClip>(BULLET_SOUND_ADDRESS);
            var laserSoundTask = _assetProvider.LoadObject<AudioClip>(LASER_SOUND_ADDRESS);
            
            var (asteroidView, bulletView, ufoView) = await UniTask.WhenAll(asteroidTask,  bulletTask, ufoTask);
            (_music, _bulletSound, _laserSound) = await UniTask.WhenAll(musicTask, bulletSoundTask, laserSoundTask);
            
            _asteroidPrefab = asteroidView.gameObject;
            _bulletPrefab = bulletView.gameObject;
            _ufoPrefab = ufoView.gameObject;
            
            IsLoaded = true;
            
            Debug.Log("[GlobalAssetCache] All assets loaded successfully.");
        }
    
    }
}
