using System.Linq;
using Kernel.Selection;
using Kernel.Types;
using Selecting.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Kernel.Targeting
{
    public class MovementCommand : MonoBehaviour
    {
        private PointObjectPool _pool;
        private UnitSelection _unitSelection;
        
        private Camera _camera;
        
        private Control _control;
        
        [Inject]
        public void Construct(Camera camera, UnitSelection unitSelection, PointObjectPool pool)
        {
            _camera = camera;
            _unitSelection = unitSelection;
            _pool = pool;
        }

        private void Awake()
        {
            _control = new Control();
        }

        private void OnEnable()
        {
            _control.Enable();
            _control.Targeting.SetTarget.started += SetTarget;
        }

        private void OnDisable()
        {
            _control.Disable();
            _control.Targeting.SetTarget.started -= SetTarget;
        }

        private void SetTarget(InputAction.CallbackContext context)
        {
            if (!_unitSelection.Selected.Any())
                return;

            var worldPoint = TryGetWorldPointUnderMouse();

            if (!worldPoint.HasValue) return;
            
            var targetPoint = _pool.PlaceTo(worldPoint.Value);
            MoveAllTo(targetPoint);
        }

        private Vector3? TryGetWorldPointUnderMouse()
        {
            var mousePosition = _control.Targeting.Position.ReadValue<Vector2>();
            var rayIntoScene =
                _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));

            if (Physics.Raycast(rayIntoScene, out var hit))
                return hit.point;
            
            return null;
        }

        private void MoveAllTo(GameObject point)
        {
            foreach (var unit in _unitSelection.Selected)
            {
                var targetable = unit.GameObject.GetComponent<ITargetable>();
                if (targetable == null) continue;
                
                if (targetable.TryAcceptPoint(point))
                    _pool.Link(point, targetable);
            }
        }
    }
}