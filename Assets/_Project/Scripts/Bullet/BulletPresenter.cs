using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class BulletPresenter : MonoBehaviour, IInitializable, IHaveDeathConditions
    {
        public event Action<BulletPresenter> OnDeath;

        [SerializeField] private BulletView _bulletView;

        private BulletModel _bulletModel;
        private WaitForSeconds _bulletLifeSpan;
        private Coroutine _coroutine;

        [Inject]
        public void Construct(BulletModel bm)
        {
            _bulletModel = bm;
        }

        public void Initialize()
        {
            _bulletLifeSpan = new WaitForSeconds(_bulletModel.BulletsLifeTime);

            _bulletView.OnHit += HandleDeath;
            _bulletView.OnEnabled += StartLifeTime;
            _bulletView.OnDisabled += OnDisable;
            _bulletView.Initialize();
        }

        public void StartLifeTime()
        {
            if (_coroutine == null)
            {
                StartCoroutine(LifeTime());
            }
        }

        public void HandleDeath()
        {
            _bulletView.StopBulletMovement();
            OnDeath?.Invoke(this);
        }

        private IEnumerator LifeTime()
        {
            _bulletView.MoveBullet(_bulletModel.MoveSpeed);
            yield return _bulletLifeSpan;
            HandleDeath();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            _bulletView.OnHit -= HandleDeath;
            _bulletView.OnEnabled -= StartLifeTime;
            _bulletView.OnDisabled -= OnDisable;
        }
    }
}