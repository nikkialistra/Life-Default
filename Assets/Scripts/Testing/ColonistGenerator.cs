using Colonists.Colonist;
using Colonists.Colonist.ColonistTypes;
using Common;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class ColonistGenerator : MonoBehaviour
    {
        private LayerMask _terrainMask;
        private ColonistFacade.Factory _colonistFactory;

        private Camera _camera;

        private PlayerInput _playerInput;
        
        private InputAction _selectAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(ColonistFacade.Factory colonistFactory, Camera camera, PlayerInput playerInput)
        {
            _colonistFactory = colonistFactory;
            _camera = camera;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");

            _selectAction = _playerInput.actions.FindAction("Select");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        private void OnEnable()
        {
            _selectAction.started += GenerateUnit;
        }

        private void OnDisable()
        {
            _selectAction.started -= GenerateUnit;
        }

        private void GenerateUnit(InputAction.CallbackContext context)
        {
            if (!Keyboard.current.altKey.isPressed)
            {
                return;
            }

            var position = _mousePositionAction.ReadValue<Vector2>();

            var ray = _camera.ScreenPointToRay(new Vector3(position.x, position.y, _camera.nearClipPlane));
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _terrainMask))
            {
                var node = AstarPath.active.GetNearest(hit.point).node;

                if (!node.Walkable)
                {
                    return;
                }
                
                _colonistFactory.Create(hit.point);
            }
        }
    }
}
