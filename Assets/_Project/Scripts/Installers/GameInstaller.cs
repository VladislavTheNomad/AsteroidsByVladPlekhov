using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GamePoolsController _gamePoolsController;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private GameProcessStarter _gameProcessStarter;
        [SerializeField] private Camera _camera;
        //[SerializeField] private GameObject _playerObject;

        [SerializeField] PlayerPresenter _playerPresenter;
        [SerializeField] PlayerView _playerView;

        [SerializeField] PlayerConfig _playerConfig;
        [SerializeField] BulletConfig _bulletConfig;
        [SerializeField] AsteroidConfig _asteroidConfig;
        [SerializeField] UFOConfig _ufoConfig;

        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>().AsSingle().WithArguments(_playerConfig).NonLazy();
            Container.Bind<BulletModel>().AsSingle().WithArguments(_bulletConfig).NonLazy();
            Container.Bind<AsteroidModel>().AsSingle().WithArguments(_asteroidConfig).NonLazy();
            Container.Bind<UfoModel>().AsSingle().WithArguments(_ufoConfig).NonLazy();

            Container.BindInterfacesAndSelfTo<UtilsCalculatePositions>().AsSingle().NonLazy();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle().NonLazy();

            //Container.BindInterfacesAndSelfTo<GameObject>().FromInstance(_playerObject).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerView>().FromInstance(_playerView).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerPresenter>().FromInstance(_playerPresenter).AsSingle();

            Container.BindInterfacesAndSelfTo<UIManager>().FromInstance(_uiManager).AsSingle();
            Container.BindInterfacesAndSelfTo<GamePoolsController>().FromInstance(_gamePoolsController).AsSingle();
            Container.BindInterfacesAndSelfTo<GameProcessStarter>().FromInstance(_gameProcessStarter).AsSingle();
        }
    }
}