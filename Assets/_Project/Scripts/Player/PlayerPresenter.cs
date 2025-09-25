using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class PlayerPresenter : MonoBehaviour, IInitializable, IHaveDeathConditions
    {
        public event Action OnRechargeTimer;
        public event Action OnAmountLaserShotChange;
        public event Action OnPlayerIsDead;

        private PlayerModel _model;
        private PlayerView _view;
        private UIManager _uiManager;

        [Inject]
        public void Construct(PlayerModel playermodel, PlayerView playerView, UIManager uiManager)
        {
            _model = playermodel;
            _view = playerView;
            _uiManager = uiManager;
        }

        private void LateUpdate()
        {
            Vector3 moveToPosition = _model.CheckBounds(_view.ViewTransform);
            if (moveToPosition != Vector3.zero)
            {
                _view.Teleport(moveToPosition);
            }
            UpdateUI();
        }

        public void Initialize()
        {
            _view.MoveRequested += AddMove;
            _view.RotateRequested += AddTorque;
            _view.FireBulletRequested += FireBullet;
            _view.FireLaserRequested += FireLaser;
            _view.CollisionDetected += HandleDeath;
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
            bool doBulletFire = _model.FireBullet(_view.ViewTransform);

            if (doBulletFire)
            {
                StartCoroutine(RechargeBullet());
            }
        }

        public void FireLaser()
        {
            _model.FireLaser(out bool canFire, out int slotIndex);

            if (canFire)
            {
                _view.ShowLaserVisual();
                StartCoroutine(RechargeLazer(slotIndex));
                _model.RayCastGo(_view.LeftBound, _view.ViewTransform);
                _model.RayCastGo(_view.RightBound, _view.ViewTransform);
            }
        }

        public void HandleDeath()
        {
            OnPlayerIsDead?.Invoke();
        }

        private IEnumerator RechargeBullet()
        {
            _model.SetCanFireBullet(false);
            yield return new WaitForSeconds(_model.BulletRechargeTime);
            _model.SetCanFireBullet(true);
        }

        private IEnumerator RechargeLazer(int index)
        {
            _model.LaserRechargeTimers[index] = _model.LaserRechargeTime;

            while (_model.LaserRechargeTimers[index] > 0f)
            {
                _model.SubtractLazerRechargeTimer(index, Time.deltaTime);

                OnRechargeTimer?.Invoke();
                yield return null;
            }
            _model.SetLazerRechargeTimer(index, 0f);
            _model.IncreaseLaserShots();
            OnAmountLaserShotChange?.Invoke();
        }

        private void UpdateUI()
        {
            _uiManager.UpdateSpeed(_model.CurrentSpeed);
            _uiManager.UpdateCoordinates(_model.Position, _model.Rotation);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            _view.MoveRequested -= AddMove;
            _view.RotateRequested -= AddTorque;
            _view.FireBulletRequested -= FireBullet;
            _view.FireLaserRequested -= FireLaser;
            _view.CollisionDetected -= HandleDeath;
        }
    }
}