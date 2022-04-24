using System;
using System.Collections;
using System.Linq;
using ColonistManagement.Targeting;
using ColonistManagement.Targeting.Formations;
using Colonists;
using Colonists.Services.Selecting;
using Enemies;
using Entities;
using Entities.Types;
using ResourceManagement;
using UI.Game;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ColonistManagement.Movement
{
    public class MovementInput : MonoBehaviour
    {
        [SerializeField] private float _mouseOffsetForRotationUpdate = 0.5f;

        private Camera _camera;

        private GameViews _gameViews;

        private SelectedColonists _selectedColonists;

        private LayerMask _rayMask;
        private LayerMask _terrainMask;

        private bool _isPositionRotating;

        private Vector2 _rotationDirection;
        private Vector2 _perpendicularDirection;

        private bool _multiCommand;
        private bool _firstCommand;

        private Coroutine _positionRotatingCoroutine;

        private PlayerInput _playerInput;

        private InputAction _multiCommandAction;

        private InputAction _moveAction;
        private InputAction _mousePositionAction;

        private InputAction _stopAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera, GameViews gameViews, SelectedColonists selectedColonists)
        {
            _playerInput = playerInput;
            _camera = camera;
            _gameViews = gameViews;
            _selectedColonists = selectedColonists;
        }

        private void Awake()
        {
            _rayMask = LayerMask.GetMask("Terrain", "Colonists", "Enemies", "Resources", "Buildings");
            _terrainMask = LayerMask.GetMask("Terrain");

            _multiCommandAction = _playerInput.actions.FindAction("Multi Command");

            _moveAction = _playerInput.actions.FindAction("Move");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");

            _stopAction = _playerInput.actions.FindAction("Stop");
        }

        public event Action<Vector3, FormationColor> PositionSet;

        public event Action<float> RotationUpdate;
        public event Action<bool, FormationColor> DestinationSet;

        public event Action MultiCommandReset;

        public event Action<Colonist> ColonistSet;
        public event Action<Unit> UnitTarget; 
        public event Action<Resource> ResourceSet;

        public event Action Stop;

        private void OnEnable()
        {
            SubscribeToActions();

            _multiCommandAction.started += StartMultiCommand;
            _multiCommandAction.canceled += StopMultiCommand;

            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();

            _multiCommandAction.started -= StartMultiCommand;
            _multiCommandAction.canceled -= StopMultiCommand;

            _stopAction.started -= OnStop;
        }

        public bool CanTarget => _selectedColonists.Colonists.Any() && !_gameViews.MouseOverUi;

        public bool MultiCommand => _multiCommand;

        public void SubscribeToActions()
        {
            _moveAction.started += SetTarget;
            _moveAction.canceled += Move;
        }

        public void UnsubscribeFromActions()
        {
            _moveAction.started -= SetTarget;
            _moveAction.canceled -= Move;
        }

        public void TargetGround(FormationColor formationColor)
        {
            if (!CanTarget)
            {
                return;
            }
            
            if (Physics.Raycast(GetRayFromMouse(), out var hit, Mathf.Infinity, _rayMask))
            {
                var ground = hit.transform.GetComponentInParent<Ground>();
                if (ground != null)
                {
                    PositionSet?.Invoke(hit.point, formationColor);
                    _isPositionRotating = true;
                    _positionRotatingCoroutine = StartCoroutine(PositionRotating(hit.point));
                }
            }
        }

        private void Move(InputAction.CallbackContext context)
        {
            Move(FormationColor.White);
        }

        public void Move(FormationColor formationColor)
        {
            if (_isPositionRotating)
            {
                if (_positionRotatingCoroutine != null)
                {
                    StopCoroutine(_positionRotatingCoroutine);
                    _positionRotatingCoroutine = null;
                }

                _isPositionRotating = false;
                var additional = _multiCommand && !_firstCommand;
                DestinationSet?.Invoke(additional, formationColor);

                _firstCommand = false;
            }
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

            TargetGround(FormationColor.White);
        }

        private bool TargetEntity()
        {
            if (Physics.Raycast(GetRayFromMouse(), out var hit, Mathf.Infinity, _rayMask))
            {
                var entity = hit.transform.GetComponentInParent<Entity>();
                if (entity != null)
                {
                    ChooseActionBasedOnEntityType(entity);
                    
                    return true;
                }
            }

            return false;
        }

        private void ChooseActionBasedOnEntityType(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Colonist:
                    ColonistSet?.Invoke(entity.Colonist);
                    break;
                case EntityType.Enemy:
                    UnitTarget?.Invoke(entity.Enemy.Unit);
                    break;
                case EntityType.Building:
                    break;
                case EntityType.Resource:
                    ResourceSet?.Invoke(entity.Resource);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartMultiCommand(InputAction.CallbackContext context)
        {
            _multiCommand = true;
            _firstCommand = true;
        }

        private void StopMultiCommand(InputAction.CallbackContext context)
        {
            _multiCommand = false;
            MultiCommandReset?.Invoke();
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
            if (Physics.Raycast(GetRayFromMouse(), out var hit, Mathf.Infinity, _terrainMask))
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
            if (Physics.Raycast(GetRayFromMouse(), out var hit, Mathf.Infinity, _terrainMask))
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

        private Ray GetRayFromMouse()
        {
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }
    }
}
