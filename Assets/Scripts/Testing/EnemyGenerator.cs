using Enemies.Enemy;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class EnemyGenerator : MonoBehaviour
    {
        private LayerMask _terrainMask;
        private EnemyFacade.Factory _enemyFactory;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _digitAction;
        private InputAction _selectAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(EnemyFacade.Factory enemyFactory, Camera camera, PlayerInput playerInput)
        {
            _enemyFactory = enemyFactory;
            _camera = camera;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");

            _selectAction = _playerInput.actions.FindAction("Select");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        private void OnEnable()
        {
            _selectAction.started += GenerateEnemy;
        }

        private void OnDisable()
        {
            _selectAction.started -= GenerateEnemy;
        }

        private void GenerateEnemy(InputAction.CallbackContext context)
        {
            if (!Keyboard.current.shiftKey.isPressed)
            {
                return;
            }

            var position = _positionAction.ReadValue<Vector2>();

            var ray = _camera.ScreenPointToRay(new Vector3(position.x, position.y, _camera.nearClipPlane));
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _terrainMask))
            {
                _enemyFactory.Create(EnemyType.Melee, hit.point);
            }
        }
    }
}
