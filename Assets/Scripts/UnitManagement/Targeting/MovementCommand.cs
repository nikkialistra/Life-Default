﻿using System.Linq;
using UI;
using Units.Services.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Targeting
{
    public class MovementCommand : MonoBehaviour
    {
        private TargetPool _pool;
        private SelectedUnits _selectedUnits;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _setTargetAction;
        private InputAction _positionAction;
        private GameViews _gameViews;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera, SelectedUnits selectedUnits, TargetPool pool, GameViews gameViews)
        {
            _playerInput = playerInput;
            _camera = camera;
            _selectedUnits = selectedUnits;
            _pool = pool;
            _gameViews = gameViews;
        }

        private void Awake()
        {
            _setTargetAction = _playerInput.actions.FindAction("SetTarget");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

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
            if (!_selectedUnits.Units.Any())
            {
                return;
            }

            var worldPoint = TryGetWorldPointUnderMouse();

            if (!worldPoint.HasValue)
            {
                return;
            }
            
            var targetPoint = _pool.PlaceTo(worldPoint.Value);
            MoveAllTo(targetPoint);
        }

        private Vector3? TryGetWorldPointUnderMouse()
        {
            if (_gameViews.MouseOverUi)
            {
                return null;
            }
            
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));

            if (Physics.Raycast(ray, out var hit))
            {
                return hit.point;
            }
            
            return null;
        }

        private void MoveAllTo(GameObject point)
        {
            foreach (var unit in _selectedUnits.Units)
            {
                var targetable = unit.GetComponent<ITargetable>();
                if (targetable == null)
                {
                    continue;
                }

                if (targetable.TryAcceptPoint(point))
                {
                    _pool.Link(point, targetable);
                }
            }
        }
    }
}