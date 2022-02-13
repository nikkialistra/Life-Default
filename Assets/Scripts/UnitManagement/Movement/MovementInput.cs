using System;
using System.Collections;
using System.Linq;
using Entities;
using UI.Game;
using UnitManagement.Targeting;
using Units.Services.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Movement
{
    public class MovementInput : MonoBehaviour
    {
        [SerializeField] private float _mouseOffsetForRotationUpdate = 0.5f;

        private Camera _camera;

        private GameViews _gameViews;

        private SelectedUnits _selectedUnits;

        private LayerMask _rayMask;
        private LayerMask _terrainMask;

        private bool _isPositionRotating;

        private Vector2 _rotationDirection;
        private Vector2 _perpendicularDirection;

        private bool _multiCommand;

        private Coroutine _positionRotatingCoroutine;

        private PlayerInput _playerInput;

        private InputAction _multiCommandAction;

        private InputAction _moveAction;
        private InputAction _positionAction;

        private InputAction _stopAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera, GameViews gameViews, SelectedUnits selectedUnits)
        {
            _playerInput = playerInput;
            _camera = camera;
            _gameViews = gameViews;
            _selectedUnits = selectedUnits;
        }

        private void Awake()
        {
            _rayMask = LayerMask.GetMask("Terrain", "Units", "Enemies", "Resources", "Buildings");
            _terrainMask = LayerMask.GetMask("Terrain");

            _multiCommandAction = _playerInput.actions.FindAction("Multi Command");

            _moveAction = _playerInput.actions.FindAction("Move");
            _positionAction = _playerInput.actions.FindAction("Position");

            _stopAction = _playerInput.actions.FindAction("Stop");
        }

        public event Action<Entity> EntitySet;

        public event Action<Vector3> PositionSet;

        public event Action<float> RotationUpdate;
        public event Action<bool> DestinationSet;

        public event Action Stop;

        private void OnEnable()
        {
            SubscribeToActions();

            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();

            _stopAction.started -= OnStop;
        }

        public bool CanTarget => _selectedUnits.Units.Any() && !_gameViews.MouseOverUi;

        public void SubscribeToActions()
        {
            _multiCommandAction.started += StartMultiCommand;
            _multiCommandAction.canceled += StopMultiCommand;

            _moveAction.started += SetTarget;
            _moveAction.canceled += Move;
        }

        public void UnsubscribeFromActions()
        {
            _multiCommandAction.started -= StartMultiCommand;
            _multiCommandAction.canceled -= StopMultiCommand;

            _moveAction.started -= SetTarget;
            _moveAction.canceled -= Move;
        }

        public bool TargetEntity()
        {
            if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _rayMask))
            {
                var entity = hit.transform.GetComponentInParent<Entity>();
                if (entity != null)
                {
                    EntitySet?.Invoke(entity);
                    return true;
                }
            }

            return false;
        }

        public void TargetGround()
        {
            if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _rayMask))
            {
                var ground = hit.transform.GetComponentInParent<Ground>();
                if (ground != null)
                {
                    PositionSet?.Invoke(hit.point);
                    _isPositionRotating = true;
                    _positionRotatingCoroutine = StartCoroutine(PositionRotating(hit.point));
                }
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (_isPositionRotating)
            {
                if (_positionRotatingCoroutine != null)
                {
                    StopCoroutine(_positionRotatingCoroutine);
                }

                _isPositionRotating = false;

                DestinationSet?.Invoke(_multiCommand);
            }
        }

        private void StartMultiCommand(InputAction.CallbackContext context)
        {
            _multiCommand = true;
        }

        private void StopMultiCommand(InputAction.CallbackContext context)
        {
            _multiCommand = false;
        }

        private void SetTarget(InputAction.CallbackContext context)
        {
            if (!CanTarget)
            {
                return;
            }

            if (TargetEntity())
            {
                return;
            }

            TargetGround();
        }

        private IEnumerator PositionRotating(Vector3 position)
        {
            while (!GotSufficientMouseOffset(position))
            {
                yield return null;
            }

            while (true)
            {
                UpdateAngle(position);

                yield return null;
            }
        }

        private bool GotSufficientMouseOffset(Vector3 position)
        {
            if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _terrainMask))
            {
                var direction = hit.point - position;
                var planeDirection = new Vector2(direction.x, direction.z);

                if (planeDirection.magnitude > _mouseOffsetForRotationUpdate)
                {
                    _rotationDirection = planeDirection;
                    _perpendicularDirection = Vector2.Perpendicular(_rotationDirection);
                    return true;
                }
            }

            return false;
        }

        private void UpdateAngle(Vector3 position)
        {
            if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _terrainMask))
            {
                var direction = hit.point - position;
                var planeDirection = new Vector2(direction.x, direction.z);

                var angle = CalculateAngle(_rotationDirection, planeDirection, _perpendicularDirection);

                RotationUpdate?.Invoke(angle);
            }
        }

        private static float CalculateAngle(Vector3 from, Vector3 to, Vector3 left)
        {
            var angle = Vector3.Angle(from, to);
            return (Vector3.Angle(left, to) > 90f) ? angle : 360 - angle;
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            Stop?.Invoke();
        }

        private Ray GetRay()
        {
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }
    }
}
