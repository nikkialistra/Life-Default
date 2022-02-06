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

        private LayerMask _entitiesMask;
        private LayerMask _terrainMask;

        private PlayerInput _playerInput;

        private InputAction _setDestinationAction;
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
            _entitiesMask = LayerMask.GetMask("Terrain", "Enemies", "Units", "Buildings");
            _terrainMask = LayerMask.GetMask("Terrain");

            _setDestinationAction = _playerInput.actions.FindAction("SetDestination");
            _positionAction = _playerInput.actions.FindAction("Position");
            _stopAction = _playerInput.actions.FindAction("Stop");
        }

        public event Action<Entity> EntitySet;

        public event Action<Vector3> PositionSet;
        public event Action<float> PositionRotationUpdate;
        public event Action PositionRotationSet;

        public event Action Stop;

        private void OnEnable()
        {
            _setDestinationAction.started += SetDestination;
            _setDestinationAction.canceled += SetPositionRotation;
            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            _setDestinationAction.started -= SetDestination;
            _setDestinationAction.canceled -= SetPositionRotation;
            _stopAction.started -= OnStop;
        }

        private void SetDestination(InputAction.CallbackContext context)
        {
            if (!_selectedUnits.Units.Any() || _gameViews.MouseOverUi)
            {
                return;
            }

            if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _entitiesMask))
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
                    _positionRotatingCoroutine = StartCoroutine(PositionRotating(hit.point));
                }
            }
        }

        private void SetPositionRotation(InputAction.CallbackContext context)
        {
            if (_positionRotatingCoroutine != null)
            {
                StopCoroutine(_positionRotatingCoroutine);
                PositionRotationSet?.Invoke();
            }
        }

        private IEnumerator PositionRotating(Vector3 position)
        {
            Vector3 rotationDirection;
            Vector3 perpendicularDirection;
            while (true)
            {
                if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _terrainMask))
                {
                    var direction = hit.point - position;
                    var planeDirection = new Vector3(direction.x, 0, direction.z);

                    if (planeDirection.magnitude > _mouseOffsetForRotationUpdate)
                    {
                        rotationDirection = planeDirection;
                        perpendicularDirection =
                            Vector2.Perpendicular(new Vector2(rotationDirection.x, rotationDirection.z));
                        break;
                    }
                }

                yield return null;
            }

            while (true)
            {
                if (Physics.Raycast(GetRay(), out var hit, Mathf.Infinity, _terrainMask))
                {
                    var direction = hit.point - position;
                    var planeDirection = new Vector3(direction.x, 0, direction.z);

                    var angle = CalculateAngle(rotationDirection, planeDirection, perpendicularDirection);
                    Debug.Log(rotationDirection + " " + planeDirection + " " + angle);

                    PositionRotationUpdate?.Invoke(angle);
                }

                yield return null;
            }
        }

        private static float CalculateAngle(Vector3 from, Vector3 to, Vector3 right)
        {
            var angle = Vector3.Angle(from, to);
            return (Vector3.Angle(right, to) > 90f) ? angle : 360 - angle;
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
