using System;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PlayerPresenter : IHaveDeathConditions, ILateTickable, IInitializable, IDisposable
    {
        private PlayerModel _model;
        private PlayerView _view;


        [Inject]
        public void Construct(PlayerModel playerModel, PlayerView playerView)
        {
            _model = playerModel;
            _view = playerView;
        }
        
        public void Initialize()
        {
            _view.MoveRequested += AddMove;
            _view.RotateRequested += AddTorque;
            _view.FireBulletRequested += FireBullet;
            _view.FireLaserRequested += FireLaser;
            _view.CollisionDetected += HandleDeath;
            _view.OnPauseClick += _model.PauseGame;

            _model.IsGamePaused += PausePlayer;
            _model.ReviveRequest += RevivePlayer;
            _model.BulletFired += _view.ShowBulletVisual;
        }

        public void LateTick()
        {
            Vector3 moveToPosition = _model.CheckBounds(_view.ViewTransform);
            if (moveToPosition != Vector3.zero)
            {
                _view.Teleport(moveToPosition);
            }

            _model.SetPosition(_view.ViewTransform.position);
            _model.SetRotation(_view.ViewTransform.eulerAngles.z); 
        }

        public void AddTorque(float direction)
        {
            _view.Rotate(direction, _model.RotationSpeed);
        }

        public void AddMove()
        {
            _view.Move(_model.MovementSpeed, ForceMode2D.Force);
            _model.UpdateSpeed(_view.Rb.linearVelocity.magnitude);
        }

        public void FireBullet()
        {
            _model.FireBullet(_view.ViewTransform);
        }

        public void FireLaser()
        {
            _model.FireLaser(out bool canFire);

            if (canFire)
            {
                _view.ShowLaserVisual();
                _model.RayCastGo(_view.LeftBound, _view.ViewTransform);
                _model.RayCastGo(_view.RightBound, _view.ViewTransform);
            }
        }

        public void HandleDeath()
        {
            if (!_model.IsInvincible)
            {
                _view.Dispose();
                _model.RequestDeathConditions();
                _view.gameObject.SetActive(false);
            }
        }

        private void PausePlayer(bool condition)
        {
            if (condition)
            {
                _view.MoveRequested -= AddMove;
                _view.RotateRequested -= AddTorque;
                _view.FireBulletRequested -= FireBullet;
                _view.FireLaserRequested -= FireLaser;
                _view.CollisionDetected -= HandleDeath;

                _view.TogglePause(true);
            }
            else
            {
                _view.MoveRequested += AddMove;
                _view.RotateRequested += AddTorque;
                _view.FireBulletRequested += FireBullet;
                _view.FireLaserRequested += FireLaser;
                _view.CollisionDetected += HandleDeath;

                _view.TogglePause(false);
            }
        }

        private void RevivePlayer()
        {
            _view.Revive();
        }

        public void Dispose()
        {
            _view.Dispose();
            _view.MoveRequested -= AddMove;
            _view.RotateRequested -= AddTorque;
            _view.FireBulletRequested -= FireBullet;
            _view.FireLaserRequested -= FireLaser;
            _view.CollisionDetected -= HandleDeath;
            _view.OnPauseClick -= _model.PauseGame;

            _model.IsGamePaused -= PausePlayer;
            _model.ReviveRequest -= RevivePlayer;
            _model.BulletFired -= _view.ShowBulletVisual;
        }
    }
}