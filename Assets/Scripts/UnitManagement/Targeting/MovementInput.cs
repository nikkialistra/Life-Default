using System;
using System.Linq;
using Entities.Entity;
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
        }

        public event Action<Entity> EntitySet;
        public event Action<Vector3> PositionSet;

        private void OnEnable()
        {
            _setDestinationAction.started += SetDestination;
        }

        private void OnDisable()
        {
            _setDestinationAction.started -= SetDestination;
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

        private Ray GetRay()
        {
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }
    }
}
