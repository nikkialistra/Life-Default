using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities
{
    public class EntitiesHovering : MonoBehaviour
    {
        private LayerMask _entitiesMask;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _positionAction;

        [Inject]
        public void Construct(Camera camera, PlayerInput playerInput)
        {
            _camera = camera;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _positionAction = _playerInput.actions.FindAction("Position");
            _entitiesMask = LayerMask.GetMask("Entities");
        }

        private void Update()
        {
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
    }
}