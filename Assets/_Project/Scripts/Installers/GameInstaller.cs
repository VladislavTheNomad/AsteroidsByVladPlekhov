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
        [SerializeField] private GameObject _playerObject;

        [SerializeField] PlayerPresenter _playerPresenter;
        [SerializeField] PlayerView _playerView;

        [SerializeField] BulletConfig _bulletConfig;

        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>().AsSingle().NonLazy();
            Container.Bind<BulletModel>().AsSingle().WithArguments(_bulletConfig.BulletsLifeTime, _bulletConfig.MoveSpeed);

            Container.BindInterfacesAndSelfTo<UtilsCalculatePositions>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerPresenter>().FromInstance(_playerPresenter).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerView>().FromInstance(_playerView).AsSingle();

            Container.BindInterfacesAndSelfTo<UIManager>().FromInstance(_uiManager).AsSingle();
            Container.BindInterfacesAndSelfTo<GamePoolsController>().FromInstance(_gamePoolsController).AsSingle();
            Container.BindInterfacesAndSelfTo<GameProcessStarter>().FromInstance(_gameProcessStarter).AsSingle();

            Container.Bind<GameObject>().FromInstance(_playerObject).AsSingle();

            //Container.Bind<FireLaser>().FromComponentOn(_playerObject).AsSingle();
            //Container.Bind<LaserVisual>().FromComponentOn(_playerObject).AsSingle();
            //Container.Bind<FireBullet>().FromComponentOn(_playerObject).AsSingle();
            Container.Bind<Rigidbody2D>().FromComponentOn(_playerObject).AsSingle();
            //Container.Bind<LineRenderer>().FromComponentOn(_playerObject).AsSingle();
            //Container.Bind<CheckDeathConditions>().FromComponentOn(_playerObject).AsSingle();

            Container.Bind<Camera>().FromInstance(_camera).AsSingle();
        }
    }
}