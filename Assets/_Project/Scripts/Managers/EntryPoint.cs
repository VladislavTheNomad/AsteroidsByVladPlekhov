using UnityEngine;
using System.Collections.Generic;

namespace Asteroids
{
    public class EntryPoint : MonoBehaviour
    {
        private const int STORAGE_OF_INITIABLES_LIST = 8;

        [SerializeField] private GameObject _initializablesList;

        private List<IInitiable> _initializables;

        private void Awake()
        {
            SetupInitializables();
        }

        private void SetupInitializables()
        {
            _initializables = new List<IInitiable>(STORAGE_OF_INITIABLES_LIST);
            _initializables.AddRange(_initializablesList.GetComponentsInChildren<IInitiable>());
            InitializeGame();
        }
        private void InitializeGame()
        {
            foreach (var item in _initializables)
            {
                item.Installation();
            }
        }
    }
}