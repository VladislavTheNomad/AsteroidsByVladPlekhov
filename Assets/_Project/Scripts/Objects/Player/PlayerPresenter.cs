using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Asteroids
{
    public class PlayerPresenter : IHaveDeathConditions, ILateTickable, ITickable
    {
        public event Action OnPlayerIsDead;
        public event Action<float> OnRechargeTimer;

        private PlayerModel _model;
        private PlayerView _view;
        private HUDModel _HUDModel;
        private SceneService _sceneService;

        [Inject]
        public void Construct(PlayerModel playerModel, PlayerView playerView, HUDModel HUDModel, SceneService sceneService)
        {
            _model = playerModel;
            _view = playerView;
            _HUDModel = HUDModel;
            _sceneService = sceneService;

            _view.MoveRequested += AddMove;
            _view.RotateRequested += AddTorque;
            _view.FireBulletRequested += FireBullet;
            _view.FireLaserRequested += FireLaser;
            _view.CollisionDetected += HandleDeath;
            _view.OnPauseClick += _sceneService.PauseGame;
            _sceneService.GameIsPaused += PausePlayer;
            _sceneService.GameIsUnpaused += UnpausePlayer;

            _HUDModel.SetMaxLaserShots(_model.GetMaxLaserShots());
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

            UpdateUI();
        }

        public void Tick()
        {
            _model.CheckBulletRecharge(Time.deltaTime);
            _model.CheckLaserRecharge(Time.deltaTime);
            OnRechargeTimer?.Invoke(_model.LaserRechargeTimers.Min());
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
            _view.Dispose();
            _view.MoveRequested -= AddMove;
            _view.RotateRequested -= AddTorque;
            _view.FireBulletRequested -= FireBullet;
            _view.FireLaserRequested -= FireLaser;
            _view.CollisionDetected -= HandleDeath;
            _view.OnPauseClick -= _sceneService.PauseGame;
            _sceneService.GameIsPaused -= PausePlayer;
            _sceneService.GameIsUnpaused -= UnpausePlayer;

            OnPlayerIsDead?.Invoke();
            _view.gameObject.SetActive(false);
        }

        private void PausePlayer()
        {
            _view.MoveRequested -= AddMove;
            _view.RotateRequested -= AddTorque;
            _view.FireBulletRequested -= FireBullet;
            _view.FireLaserRequested -= FireLaser;
            _view.CollisionDetected -= HandleDeath;

            _view.TogglePause(true);
        }

        private void UnpausePlayer()
        {
            _view.MoveRequested += AddMove;
            _view.RotateRequested += AddTorque;
            _view.FireBulletRequested += FireBullet;
            _view.FireLaserRequested += FireLaser;
            _view.CollisionDetected += HandleDeath;

            _view.TogglePause(false);
        }

        private void UpdateUI()
        {
            _HUDModel.UpdateCurrentShots(_model.CurrentLaserShots);
            _HUDModel.UpdateSpeed(_model.CurrentSpeed);
            _HUDModel.UpdateCoordinates(_model.Position, _model.Rotation);
        }
    }
}