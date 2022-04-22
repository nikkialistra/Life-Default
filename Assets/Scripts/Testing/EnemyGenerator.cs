﻿using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class EnemyGenerator : MonoBehaviour
    {
        private LayerMask _terrainMask;
        private Enemy.Factory _enemyFactory;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _digitAction;
        private InputAction _selectAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(Enemy.Factory enemyFactory, Camera camera, PlayerInput playerInput)
        {
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
            _selectAction.started += GenerateEnemy;
        }

        private void OnDisable()
        {
            _selectAction.started -= GenerateEnemy;
        }

        private void GenerateEnemy(InputAction.CallbackContext context)
        {
            if (!Keyboard.current.ctrlKey.isPressed)
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
                
                var enemy = _enemyFactory.Create();
                enemy.SetAt(hit.point);
            }
        }
    }
}
