using UnityEngine;

namespace Asteroids
{
    public class AsteroidConfig : MonoBehaviour
    {
        [field: SerializeField, Range(10, 100)] public float MaxMoveSpeed { get; private set; }
        [field: SerializeField, Range(10, 100)] public float MinMoveSpeed { get; private set; }
        [field: SerializeField] public float AccelerationModificator { get; private set; }
        [field: SerializeField, Range(1, 10)] public int MaxSizeLevel { get; private set; }
        [field: SerializeField, Range(1, 10)] public int SmallAsteroidQuantity { get; private set; }
        [field: SerializeField, Range(1, 5)] public int ScorePoints { get; private set; }
    }
}