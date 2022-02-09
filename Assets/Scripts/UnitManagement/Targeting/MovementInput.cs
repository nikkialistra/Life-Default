using System;
using System.Collections;
using System.Linq;
using Entities;
using UI.Game;
using Units.Services.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Targeting
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
        private PlayerInput _playerInput;

        private InputAction _setDestinationAction;
        private InputAction _addDestinationAction;
        private InputAction _positionAction;
        private InputAction _stopAction;

        private Coroutine _positionRotatingCoroutine;

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

            _setDestinationAction = _playerInput.actions.FindAction("SetDestination");
            _addDestinationAction = _playerInput.actions.FindAction("AddDestination");
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
            _setDestinationAction.started += SetTarget;
            _setDestinationAction.canceled += SetDestination;
            _addDestinationAction.canceled += AddDestination;
            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            _setDestinationAction.started -= SetTarget;
            _setDestinationAction.canceled -= SetDestination;
            _addDestinationAction.canceled -= AddDestination;
            _stopAction.started -= OnStop;
        }

        private void SetTarget(InputAction.CallbackContext context)
        {
            if (!_selectedUnits.Units.Any() || _gameViews.MouseOverUi)
            {
                return;
            }

            if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _rayMask))
            {
                var entity = hit.transform.GetComponentInParent<Entity>();
                if (entity != null)
                {
                    EntitySet?.Invoke(entity);
                    return;
                }

                var ground = hit.transform.GetComponentInParent<Ground>();
                if (ground != null)
                {
                    PositionSet?.Invoke(hit.point);
                    _isPositionRotating = true;
                    _positionRotatingCoroutine = StartCoroutine(PositionRotating(hit.point));
                }
            }
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

        private void SetDestination(InputAction.CallbackContext context)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                return;
            }

            if (_isPositionRotating)
            {
                if (_positionRotatingCoroutine != null)
                {
                    StopCoroutine(_positionRotatingCoroutine);
                }

                _isPositionRotating = false;
                DestinationSet?.Invoke(false);
            }
        }

        private void AddDestination(InputAction.CallbackContext context)
        {
            if (_isPositionRotating)
            {
                if (_positionRotatingCoroutine != null)
                {
                    StopCoroutine(_positionRotatingCoroutine);
                }

                _isPositionRotating = false;
                DestinationSet?.Invoke(true);
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
