using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ProjectBootstrap>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneService>().AsSingle();
            Container.BindExecutionOrder<ProjectBootstrap>(-90);
            
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<GlobalAssetCache>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<CloudData>().AsSingle();
            Container.BindInterfacesAndSelfTo<PauseGame>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreCounter>().AsSingle();
            Container.BindInterfacesAndSelfTo<IAPProductList>().AsSingle();
            Container.BindInterfacesAndSelfTo<IAPService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UnityAdService>().AsSingle();
            Container.BindInterfacesAndSelfTo<StoredDataHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<StoredDataNames>().AsSingle();
            Container.BindInterfacesAndSelfTo<DataToSave>().AsSingle();

            var configs = new GameConfigs();
            configs.DestructableLayers = LayerMask.GetMask("Enemy");
            Container.Bind<GameConfigs>().FromInstance(configs).AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle();
        }
    }
}