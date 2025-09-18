using UnityEngine;

namespace Asteroids
{
    public class BulletConfig : MonoBehaviour
    {
        [field: SerializeField] public float BulletsLifeTime {get; private set;}
        [field: SerializeField] public float MoveSpeed { get; private set; }

    }
}
