
using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{

    public class SoundService : MonoBehaviour, IInitializable, IDisposable
    {
        private AudioClip _music;
        private AudioClip _laserFireSound;
        private AudioClip _bulletFireSound;
        
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        
        private PlayerModel _playerModel;
        private GlobalAssetCache _globalAssetCache;

        [Inject]
        public void Construct(PlayerModel playerModel, GlobalAssetCache globalAssetCache)
        {
            _playerModel =  playerModel;
            _globalAssetCache =  globalAssetCache;
        }
        
        public void Initialize()
        {
            _music = _globalAssetCache.GetMusic();
            _bulletFireSound = _globalAssetCache.GetBulletSound();
            _laserFireSound = _globalAssetCache.GetLaserSound();
            
            _playerModel.LaserFired += PlayLaserSound;
            _playerModel.BulletFired += PlayBulletSound;
        }
        
        public void Start()
        {
            _musicSource.clip =  _music;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        private void PlayBulletSound() => _sfxSource.PlayOneShot(_bulletFireSound);

        private void PlayLaserSound() => _sfxSource.PlayOneShot(_laserFireSound);
        

        public void Dispose()
        {
            _playerModel.LaserFired -= PlayLaserSound;
            _playerModel.BulletFired -= PlayBulletSound;
        }
    }
}
