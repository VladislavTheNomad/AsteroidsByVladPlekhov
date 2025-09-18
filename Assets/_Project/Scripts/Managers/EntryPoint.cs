//using UnityEngine;
//using System.Collections.Generic;
//using Zenject;

//namespace Asteroids
//{
//    public class EntryPoint : MonoBehaviour
//    {
//        private List<IInitiable> _initializables;

//        [Inject]
//        public void Construct(List<IInitiable> initializables)
//        {
//            _initializables = initializables;
//            InitializeGame();
//        }

//        private void InitializeGame()
//        {
//            foreach (var item in _initializables)
//            {
//                item.Installation();
//            }
//        }
//    }
//}