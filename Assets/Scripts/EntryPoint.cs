using UnityEngine;
using System.Linq;

namespace Asteroids
{
    public class EntryPoint : MonoBehaviour
    {
        private IInitiable[] initiables;

        private void Awake()
        {
            initiables = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Where(x => x is IInitiable).Select(x => x as IInitiable).OrderBy(x => x.sortingIndex).ToArray();

            foreach (var initiable in initiables)
            {
                initiable.Installation();
            }
        }

     
    }
}
