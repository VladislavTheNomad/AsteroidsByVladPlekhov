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
        private GameObject _playerObject;
        private Vector3 _destination;
        private bool _initialized;

        [Inject]
        public void Construct(UfoModel model, GameObject playerObject)
        {
            _model = model;
            _playerObject = playerObject;
        }

        private void OnEnable()
        {
            if(_initialized)
            {
                MoveBehaviour();
            }
        }

        public void Initialize()
        {
            _view.OnDeath += DeathConditions;
            _view.Initialize();
            _initialized = true;
            MoveBehaviour();
        }

        public async void MoveBehaviour()
        {
            while (gameObject.activeSelf)
            {
                await Task.Delay((int)(_model.GapBetweenPositionChanging * 1000));
                if (_view == null || _playerObject == null || !_playerObject.activeSelf)
                {
                    return;
                }
                _destination = (_playerObject.transform.position - _view.transform.position).normalized;
                _view.DoMove(_destination, _model.MoveSpeed);
            }
        }

        public void DeathConditions()
        {
            OnDeathTakeScore?.Invoke(_model.ScorePoints);
            OnDeath?.Invoke(this);
        }

        public void OnDestroy()
        {
            _view.OnDeath -= MoveBehaviour;
        }
    }
}