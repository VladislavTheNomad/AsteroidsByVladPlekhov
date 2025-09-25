using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids
{
    public class UfoPresenter : MonoBehaviour, IGetPointsOnDestroy, IHaveDeathConditions
    {
        public event Action<int> OnDeathTakeScore;
        public event Action<UfoPresenter> OnDeath;

        [SerializeField] private UfoView _view;

        private UfoModel _model;
        private bool _initialized;
        private Vector3 _destination;

        [Inject]
        public void Construct(UfoModel model)
        {
            _model = model;
        }

        private void OnEnable()
        {
            if(_initialized)
            {
                ApplyBehaviour();
            }
        }

        public void Initialize()
        {
            _view.OnDeath += HandleDeath;
            _view.Initialize();
            _initialized = true;
            ApplyBehaviour();
        }

        public async void ApplyBehaviour()
        {
            while (_view.gameObject.activeSelf)
            {
                await Task.Delay((int)(_model.GapBetweenPositionChanging * 1000));
                if (_model.CheckNull(_view.ViewTransform))
                {
                    _destination = _model.GetNewDestination(_view.ViewTransform);
                    _view.Move(_destination, _model.MoveSpeed);
                }
                else
                {
                    break;
                }
            }
        }

        public void HandleDeath()
        {
            OnDeathTakeScore?.Invoke(_model.ScorePoints);
            OnDeath?.Invoke(this);
        }

        public void OnDestroy()
        {
            _view.OnDeath -= HandleDeath;
        }
    }
}