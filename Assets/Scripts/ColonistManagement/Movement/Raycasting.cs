using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ColonistManagement.Movement
{
    public class Raycasting : MonoBehaviour
    {
        public LayerMask RayMask => _rayMask;
        public LayerMask TerrainMask => _terrainMask;

        private Camera _camera;

        private LayerMask _rayMask;
        private LayerMask _terrainMask;

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
            _rayMask = LayerMask.GetMask("Terrain", "Colonists", "Enemies", "Resources", "Buildings");
            _terrainMask = LayerMask.GetMask("Terrain");

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
