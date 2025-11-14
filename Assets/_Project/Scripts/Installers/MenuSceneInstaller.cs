using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class MenuSceneInstaller : MonoInstaller
    {
        [SerializeField] private MenuUIView _menuUIView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MenuUIView>().FromInstance(_menuUIView).AsSingle();
            Container.BindInterfacesAndSelfTo<MenuUiModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuUIPresenter>().AsSingle();
        }
    }
}
