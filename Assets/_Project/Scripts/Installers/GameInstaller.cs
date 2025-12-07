using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CloudData>().AsSingle();
            Container.BindInterfacesAndSelfTo<PauseGame>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreCounter>().AsSingle();
            Container.BindInterfacesAndSelfTo<IAPProductList>().AsSingle();
            Container.BindInterfacesAndSelfTo<IAPService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UnityAdService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<StoredDataHandler>().AsSingle().NonLazy();

            var configs = new GameConfigs();
            configs.DestructableLayers = LayerMask.GetMask("Enemy");
            Container.Bind<GameConfigs>().FromInstance(configs).AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle();
        }
    }
}