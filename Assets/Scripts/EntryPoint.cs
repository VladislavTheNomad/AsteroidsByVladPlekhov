using UnityEngine;
using System.Linq;

namespace Asteroids
{
    public class EntryPoint : MonoBehaviour
    {

        private void Awake()
        {
            var initiables = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IInitiable>().OrderBy(x=> x.sortingIndex);

            foreach (var initiable in initiables)
            {
                initiable.Installation();
            }
        }
    }
}
