using System.Linq;
using Game.Units.Services;
using Kernel.Selection;
using Kernel.Types;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Kernel.Targeting
{
    public class MovementCommand : MonoBehaviour
    {
        private TargetPool _pool;
        private UnitSelection _unitSelection;
        
        private Camera _camera;
        
        private PlayerInput _playerInput;
        
        private InputAction _setTargetAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera, UnitSelection unitSelection, TargetPool pool)
        {
            _playerInput = playerInput;
            _camera = camera;
            _unitSelection = unitSelection;
            _pool = pool;
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
            if (!_unitSelection.Selected.Any())
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
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var rayIntoScene =
                _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));

            if (Physics.Raycast(rayIntoScene, out var hit))
            {
                return hit.point;
            }
            
            return null;
        }

        private void MoveAllTo(GameObject point)
        {
            foreach (var unit in _unitSelection.Selected)
            {
                var targetable = unit.GameObject.GetComponent<ITargetable>();
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