using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _initializablesList;

        public override void InstallBindings()
        {
            foreach (var initializable in _initializablesList.GetComponentsInChildren<IInitiable>())
            {
                Container.Bind<IInitiable>().To(initializable.GetType()).FromInstance(initializable).AsSingle();
            }
        }
    }
}