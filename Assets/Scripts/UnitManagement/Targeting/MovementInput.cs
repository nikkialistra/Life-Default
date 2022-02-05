using System;
using System.Linq;
using Entities;
using UI.Game;
using Units.Services.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Targeting
{
    public class MovementInput : MonoBehaviour
    {
        private Camera _camera;

        private GameViews _gameViews;

        private SelectedUnits _selectedUnits;

        private PlayerInput _playerInput;

        private InputAction _setDestinationAction;
        private InputAction _positionAction;
        private InputAction _stopAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera, GameViews gameViews, SelectedUnits selectedUnits)
        {
            _playerInput = playerInput;
            _camera = camera;
            _gameViews = gameViews;
            _selectedUnits = selectedUnits;
        }

        private void Awake()
        {
            _setDestinationAction = _playerInput.actions.FindAction("SetDestination");
            _positionAction = _playerInput.actions.FindAction("Position");
            _stopAction = _playerInput.actions.FindAction("Stop");
        }

        public event Action<Entity> EntitySet;
        public event Action<Vector3> PositionSet;

        public event Action Stop;

        private void OnEnable()
        {
            _setDestinationAction.started += SetDestination;
            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            _setDestinationAction.started -= SetDestination;
            _stopAction.started -= OnStop;
        }

        private void SetDestination(InputAction.CallbackContext context)
        {
            if (!_selectedUnits.Units.Any() || _gameViews.MouseOverUi)
            {
                return;
            }

            if (Physics.Raycast(GetRay(), out var hit))
            {
                var entity = hit.transform.GetComponentInParent<Entity>();
                if (entity != null)
                {
                    EntitySet?.Invoke(entity);
                    return;
                }

                var ground = hit.transform.GetComponentInParent<Ground>();
                if (ground != null)
                {
                    PositionSet?.Invoke(hit.point);
                }
            }
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            Stop?.Invoke();
        }

        private Ray GetRay()
        {
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }
    }
}
