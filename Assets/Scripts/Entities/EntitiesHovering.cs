using UnitManagement.Selection;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities
{
    public class EntitiesHovering : MonoBehaviour
    {
        private bool _canHover = true;

        private LayerMask _entitiesMask;
        private Camera _camera;
        private SelectionInput _selectionInput;

        private PlayerInput _playerInput;

        private InputAction _positionAction;

        [Inject]
        public void Construct(Camera camera, SelectionInput selectionInput, PlayerInput playerInput)
        {
            _camera = camera;
            _selectionInput = selectionInput;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _positionAction = _playerInput.actions.FindAction("Position");
            _entitiesMask = LayerMask.GetMask("Entities");
        }

        private void OnEnable()
        {
            _selectionInput.Selecting += OnSelecting;
            _selectionInput.SelectingEnd += OnSelectingEnd;
        }

        private void OnDisable()
        {
            _selectionInput.Selecting -= OnSelecting;
            _selectionInput.SelectingEnd -= OnSelectingEnd;
        }

        private void Update()
        {
            if (!_canHover)
            {
                return;
            }

            var point = _positionAction.ReadValue<Vector2>();

            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _entitiesMask))
            {
                if (hit.transform.TryGetComponent(out IHoverable hoverable))
                {
                    hoverable.OnHover();
                }
            }
        }

        private void OnSelecting(Rect _)
        {
            _canHover = false;
        }

        private void OnSelectingEnd(Rect _)
        {
            _canHover = true;
        }
    }
}