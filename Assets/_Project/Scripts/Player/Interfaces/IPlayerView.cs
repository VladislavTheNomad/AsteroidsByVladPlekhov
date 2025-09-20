using System;
using UnityEngine;

namespace Asteroids
{
    public interface IPlayerView
    {
        event Action MoveRequested;
        event Action<float> RotateRequested;
        event Action FireBulletRequested;
        event Action FireLaserRequested;
        event Action CollisionDetected;

        Rigidbody2D Rb { get; }
        GameObject LeftBound { get; }
        GameObject RightBound { get; }

        Transform transform { get; }

        void ShowLaserVisual();
        void SetLineRenderer();
    }
}