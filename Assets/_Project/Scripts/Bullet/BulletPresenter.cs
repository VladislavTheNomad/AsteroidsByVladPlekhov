using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class BulletPresenter : MonoBehaviour, IInitializable
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

            _bulletView.OnHit += DeathConditions;
            _bulletView.OnEnabled += StartLifeTime;
            _bulletView.OnDisabled += OnDestroy;
            _bulletView.Initialize();
        }

        public void StartLifeTime()
        {
            if (_coroutine == null)
            {
                StartCoroutine(LifeTime());
            }
        }

        public void DeathConditions()
        {
            _bulletView.StopBulletMovement();
            OnDeath?.Invoke(this);
        }

        private IEnumerator LifeTime()
        {
            _bulletView.MoveBullet(_bulletModel.MoveSpeed);
            yield return _bulletLifeSpan;
            DeathConditions();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            _bulletView.OnHit -= DeathConditions;
            _bulletView.OnEnabled -= StartLifeTime;
            _bulletView.OnDisabled -= OnDisable;
        }
    }
}