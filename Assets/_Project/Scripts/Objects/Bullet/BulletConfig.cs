using UnityEngine;

namespace Asteroids
{
    [CreateAssetMenu(fileName = "BulletConfig", menuName = "Configs/BulletConfig")]
    public class BulletConfig : ScriptableObject
    {
        [field: SerializeField] public float BulletsLifeTime {get; private set;}
        [field: SerializeField] public float MoveSpeed { get; private set; }
    }
}