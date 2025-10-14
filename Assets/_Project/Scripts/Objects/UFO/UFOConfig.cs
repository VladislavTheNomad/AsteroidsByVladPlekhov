using UnityEngine;

namespace Asteroids
{
    public class UFOConfig : MonoBehaviour
    {
        [field: SerializeField, Range(1, 5)] public int ScorePoints { get; private set; }
        [field: SerializeField, Range(1, 5)] public int MoveSpeed { get; private set; }
        [field: SerializeField, Range(1, 5)] public int GapBetweenPositionChanging { get; private set; }
    }
}