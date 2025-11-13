using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PauseGame>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreCounter>().AsSingle().NonLazy();
            Container.Bind<IAdService>().To<UnityAdService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<IAPService>().AsSingle().NonLazy();
        }
    }
}