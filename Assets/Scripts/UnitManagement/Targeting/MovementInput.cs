using System;
using System.Linq;
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

        private InputAction _setTargetAction;
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
            _setTargetAction = _playerInput.actions.FindAction("SetTarget");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        public event Action<TargetObject, RaycastHit> TargetSet;

        private void OnEnable()
        {
            _setTargetAction.started += SetTarget;
        }

        private void OnDisable()
        {
            _setTargetAction.started -= SetTarget;
        }

        private void SetTarget(InputAction.CallbackContext context)
        {
            if (!_selectedUnits.Units.Any() || _gameViews.MouseOverUi)
            {
                return;
            }

            var ray = GetRay();
            if (Physics.Raycast(ray, out var hit))
            {
                var targetObject = hit.transform.GetComponentInParent<TargetObject>();
                if (targetObject != null)
                {
                    TargetSet?.Invoke(targetObject, hit);
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