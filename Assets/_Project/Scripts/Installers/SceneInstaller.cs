using UnityEngine;
using Zenject;


namespace Asteroids
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private HUDView _HUDView;
        [SerializeField] private GameProcessStarter _gameProcessStarter;
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private SoundService _soundService;
        [SerializeField] private int _poolSize;

        public override void InstallBindings()
        {
            var globalAssetCache = Container.Resolve<GlobalAssetCache>();

            GameObject astroidPrefab = globalAssetCache.GetAsteroidPrefab();
            GameObject bulletPrefab = globalAssetCache.GetBulletPrefab();
            GameObject ufoPrefab = globalAssetCache.GetUFOPrefab();

            Container.BindMemoryPool<AsteroidView, MonoMemoryPool<AsteroidView>>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(astroidPrefab).
                UnderTransformGroup("Asteroids");

            Container.BindMemoryPool<BulletView, BulletPool>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(bulletPrefab).
                UnderTransformGroup("Bullets");

            Container.BindMemoryPool<UfoView, MonoMemoryPool<UfoView>>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(ufoPrefab).
                UnderTransformGroup("UFO's");

            Container.Bind<AsteroidFactory>().AsSingle();
            Container.Bind<BulletFactory>().AsSingle();
            Container.Bind<UfoFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<UtilsCalculatePositions>().AsSingle();
            Container.BindInterfacesAndSelfTo<Statistics>().AsSingle();
            Container.Bind<IAnalytics>().To<FirebaseAnalyticsService>().AsSingle();

            Container.BindInterfacesAndSelfTo<HUDModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BulletModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AsteroidModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UfoModel>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<AsteroidPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<UfoPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<BulletPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<HUDPresenter>().AsSingle();

            Container.BindInterfacesAndSelfTo<HUDView>().FromInstance(_HUDView).AsSingle();
            Container.BindInterfacesAndSelfTo<GameProcessStarter>().FromInstance(_gameProcessStarter).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerView>().FromInstance(_playerView).AsSingle();
            Container.BindInterfacesAndSelfTo<SoundService>().FromInstance(_soundService).AsSingle();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle();
        }
    }
}