using Colonists;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class UnitPlacer : MonoBehaviour
    {
        private LayerMask _terrainMask;
        private Colonist.Factory _colonistFactory;
        private Enemy.Factory _enemyFactory;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _selectAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(Colonist.Factory colonistFactory, Enemy.Factory enemyFactory, Camera camera, PlayerInput playerInput)
        {
            _colonistFactory = colonistFactory;
            _enemyFactory = enemyFactory;
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
            _selectAction.started += Generate;
        }

        private void OnDisable()
        {
            _selectAction.started -= Generate;
        }

        private void Generate(InputAction.CallbackContext context)
        {
            if (!Keyboard.current.altKey.isPressed && !Keyboard.current.ctrlKey.isPressed)
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

                GenerateUnit(hit);
            }
        }

        private void GenerateUnit(RaycastHit hit)
        {
            if (Keyboard.current.altKey.isPressed)
            {
                var colonist = _colonistFactory.Create();
                colonist.SetAt(hit.point);
            }
            else if (Keyboard.current.ctrlKey.isPressed)
            {
                var enemy = _enemyFactory.Create();
                enemy.SetAt(hit.point);
            }
        }
    }
}
