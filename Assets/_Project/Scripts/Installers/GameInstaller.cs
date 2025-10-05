using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameHUDManager _uiManager;
        [SerializeField] private GameProcessStarter _gameProcessStarter;
        [SerializeField] private Camera _camera;

        [SerializeField] private PlayerView _playerView;

        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private BulletConfig _bulletConfig;
        [SerializeField] private AsteroidConfig _asteroidConfig;
        [SerializeField] private UFOConfig _ufoConfig;

        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _ufoPrefab;
        [SerializeField] private int _poolSize;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<AsteroidView, AsteroidPool>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(_asteroidPrefab).
                UnderTransformGroup("Asteroids");

            Container.BindMemoryPool<BulletView, BulletPool>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(_bulletPrefab).
                UnderTransformGroup("Bullets");

            Container.BindMemoryPool<UfoView, UfoPool>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(_ufoPrefab).
                UnderTransformGroup("UFO's");

            Container.Bind<AsteroidFactory>().AsSingle();
            Container.Bind<BulletFactory>().AsSingle();
            Container.Bind<UfoFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<GameHUDManager>().FromInstance(_uiManager).AsSingle();
            Container.BindInterfacesAndSelfTo<GameProcessStarter>().FromInstance(_gameProcessStarter).AsSingle();

            Container.Bind<PlayerModel>().AsSingle().WithArguments(_playerConfig).NonLazy();
            Container.Bind<BulletModel>().AsSingle().WithArguments(_bulletConfig).NonLazy();
            Container.Bind<AsteroidModel>().AsSingle().WithArguments(_asteroidConfig).NonLazy();
            Container.Bind<UfoModel>().AsSingle().WithArguments(_ufoConfig).NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerView>().FromInstance(_playerView).AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<AsteroidPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<UfoPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<BulletPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerPresenter>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<UtilsCalculatePositions>().AsSingle().NonLazy();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle().NonLazy();
        }
    }
}