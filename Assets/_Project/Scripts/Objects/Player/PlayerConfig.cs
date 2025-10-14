using UnityEngine;

namespace Asteroids
{
    public class PlayerConfig : MonoBehaviour
    {
        [field: SerializeField, Range(0, 10)] public int MaxLaserShots { get; private set; }
        [field: SerializeField, Range(0, 100)] public float LaserDistance { get; private set; }
        [field: SerializeField, Range(10, 100)] public float RotationSpeed { get; private set; }
        [field: SerializeField, Range(1, 20)] public float MovementSpeed { get; private set; }
        [field: SerializeField, Range(0, 1)] public float BulletRechargeTime { get; private set; }
        [field: SerializeField, Range(1, 30)] public float LaserRechargeTime { get; private set; }
        [field: SerializeField] public LayerMask DestructableLayers { get; private set; }
    }
}