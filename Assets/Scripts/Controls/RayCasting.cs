using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls
{
    public class RayCasting : MonoBehaviour
    {
        public LayerMask UnitsMask => _unitsMask;
        public LayerMask EntitiesMask => _entitiesMask;
        public LayerMask TerrainMask => _terrainMask;

        [SerializeField] private LayerMask _unitsMask;
        [SerializeField] private LayerMask _entitiesMask;
        [SerializeField] private LayerMask _terrainMask;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera)
        {
            _playerInput = playerInput;
            _camera = camera;
        }

        private void Awake()
        {
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        public Ray GetRayFromMouse()
        {
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }
    }
}
