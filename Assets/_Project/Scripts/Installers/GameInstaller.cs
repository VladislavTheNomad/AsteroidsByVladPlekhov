using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PauseGame>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreCounter>().AsSingle();
            Container.Bind<ProductList>().AsSingle();
            Container.BindInterfacesAndSelfTo<IAPService>().AsSingle();
            Container.Bind<IAdService>().To<UnityAdService>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveData>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameConfigs>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle();
        }
    }
}