using System.Linq;
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
            if (!_selectedUnits.Units.Any() || _gameViews.MouseOverUi)
            {
                return;
            }
            
            var ray = GetRay();
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out TargetObject targetObject))
                {
                    MoveAll(targetObject, hit);
                }
            }
        }

        private Ray GetRay()
        {
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }

        private void MoveAll(TargetObject targetObject, RaycastHit hit)
        {
            if (targetObject.HasDestinationPoint)
            {
                targetObject.ShowIndicator();
                var destinationPoint = targetObject.GetDestinationPoint();
                var target = _pool.PlaceTo(destinationPoint);
                MoveAllTo(target);
            }
            else
            {
                var target = _pool.PlaceTo(hit.point);
                MoveAllTo(target);
            }
        }

        private void MoveAllTo(Target target)
        {
            foreach (var unit in _selectedUnits.Units)
            {
                var targetable = unit.GetComponent<ITargetable>();
                if (targetable == null)
                {
                    continue;
                }

                if (targetable.TryAcceptTarget(target))
                {
                    _pool.Link(target, targetable);
                }
            }
        }
    }
}