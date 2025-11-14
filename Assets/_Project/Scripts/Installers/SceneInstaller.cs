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

        [SerializeField] private string _asteroidPrefabAddress = "Asteroid";
        [SerializeField] private string _bulletPrefabAddress = "Bullet";
        [SerializeField] private string _ufoPrefabAddress = "Ufo";
        [SerializeField] private int _poolSize;

        public override void InstallBindings()
        {
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle().NonLazy();
            
            var assetProvider = Container.Resolve<IAssetProvider>();

            var asteroidView = assetProvider.Load<AsteroidView>(_asteroidPrefabAddress);
            var bulletView = assetProvider.Load<BulletView>(_bulletPrefabAddress);
            var ufoView = assetProvider.Load<UfoView>(_ufoPrefabAddress);

            Container.BindMemoryPool<AsteroidView, MonoMemoryPool<AsteroidView>>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(asteroidView.gameObject).
                UnderTransformGroup("Asteroids");

            Container.BindMemoryPool<BulletView, BulletPool>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(bulletView.gameObject).
                UnderTransformGroup("Bullets");

            Container.BindMemoryPool<UfoView, MonoMemoryPool<UfoView>>().
                WithInitialSize(_poolSize).
                FromComponentInNewPrefab(ufoView.gameObject).
                UnderTransformGroup("UFO's");

            Container.Bind<AsteroidFactory>().AsSingle();
            Container.Bind<BulletFactory>().AsSingle();
            Container.Bind<UfoFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<UtilsCalculatePositions>().AsSingle().NonLazy();
            //Container.BindInterfacesAndSelfTo<SaveData>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneService>().AsSingle().NonLazy();
            Container.Bind<Statistics>().AsSingle().NonLazy();
            Container.Bind<IAnalytics>().To<FirebaseAnalyticsService>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<HUDModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BulletModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AsteroidModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UfoModel>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<AsteroidPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<UfoPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<BulletPresenter>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HUDPresenter>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<HUDView>().FromInstance(_HUDView).AsSingle();
            Container.BindInterfacesAndSelfTo<GameProcessStarter>().FromInstance(_gameProcessStarter).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerView>().FromInstance(_playerView).AsSingle().NonLazy();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle().NonLazy();
        }
    }
}