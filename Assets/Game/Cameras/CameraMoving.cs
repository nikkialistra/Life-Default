using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraMoving : MonoBehaviour
    {
        [Title("Movement")] 
        [MinValue(0)] 
        [SerializeField] private float _movementNormalSpeed;
        [MinValue(0)] 
        [SerializeField] private float _movementFastSpeed;
        [MinValue(0)] 
        [SerializeField] private float _dragMultiplier;
        
        [Title("Boundaries")]
        [ValidateInput("@_minimumPositionX < _maximumPositionX", "Minimum position should be less than maximum position")]
        [SerializeField] private float _minimumPositionX;
        [ValidateInput("@_minimumPositionX < _maximumPositionX", "Minimum position should be less than maximum position")]
        [SerializeField] private float _maximumPositionX;
        [Space]
        [ValidateInput("@_minimumPositionZ < _maximumPositionZ", "Minimum position should be less than maximum position")]
        [SerializeField] private float _minimumPositionZ;
        [ValidateInput("@_minimumPositionZ < _maximumPositionZ", "Minimum position should be less than maximum position")]
        [SerializeField] private float _maximumPositionZ;
        
        public event Action<Vector3> PositionUpdate;
        
        public Vector3 Position { get; set; }

        private Camera _camera;
        
        private float _movementSpeed;
        
        private Vector3? _dragStartPosition;
        private Vector3 _dragCurrentPosition;

        private Coroutine _moveCoroutine;
        private Coroutine _dragCoroutine;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _fastMovementAction;
        private InputAction _dragAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            
            _movementAction = _playerInput.actions.FindAction("Movement");
            _fastMovementAction = _playerInput.actions.FindAction("FastMovement");
            _dragAction = _playerInput.actions.FindAction("Drag");
            _positionAction = _playerInput.actions.FindAction("Position");
        }
        
        private void OnEnable()
        {
            _movementAction.started += MovementStart;
            _movementAction.canceled += MovementStop;

            _fastMovementAction.started += FastMovementOn;
            _fastMovementAction.canceled += FastMovementOff;
            
            _dragAction.started += DragStart;
            _dragAction.canceled += DragStop;
        }
        
        private void OnDisable()
        {
            _movementAction.started -= MovementStart;
            _movementAction.canceled -= MovementStop;

            _fastMovementAction.started -= FastMovementOn;
            _fastMovementAction.canceled -= FastMovementOff;
            
            _dragAction.started -= DragStart;
            _dragAction.canceled -= DragStop;
        }

        private void Start()
        {
            _movementSpeed = _movementNormalSpeed;
        }

        private void MovementStart(InputAction.CallbackContext context)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (true)
            {
                var movementInput = _movementAction.ReadValue<Vector2>();
                var movement = movementInput * (_movementSpeed * Time.deltaTime);
                UpdatePosition(movement);
                
                yield return null;
            }
        }

        private void UpdatePosition(Vector2 movement)
        {
            var position = Position;
            position += transform.right * movement.x;
            position += transform.up * movement.y + transform.forward * movement.y;

            ClampPositionByConstraints(position);
        }

        private void ClampPositionByConstraints(Vector3 position)
        {
            
            
            if (position.x < _minimumPositionX)
            {
                position.x = _minimumPositionX;
            }
            
            if (position.x > _maximumPositionX)
            {
                position.x = _maximumPositionX;
            }
            
            if (position.z < _minimumPositionZ)
            {
                position.z = _minimumPositionZ;
            }
            
            if (position.z > _maximumPositionZ)
            {
                position.z = _maximumPositionZ;
            }
            
            Position = position;
            PositionUpdate?.Invoke(Position);
        }

        private void MovementStop(InputAction.CallbackContext context)
        {
            if (_moveCoroutine == null)
            {
                throw new InvalidOperationException();
            }
            StopCoroutine(_moveCoroutine);
        }

        private void FastMovementOn(InputAction.CallbackContext context)
        {
            _movementSpeed = _movementFastSpeed;
        }

        private void FastMovementOff(InputAction.CallbackContext context)
        {
            _movementSpeed = _movementNormalSpeed;
        }
        
        private void DragStart(InputAction.CallbackContext context)
        {
            var plane = new Plane(Vector3.up, Vector3.zero);

            var ray = _camera.ScreenPointToRay(_positionAction.ReadValue<Vector2>());

            if (plane.Raycast(ray, out var entry))
            {
                _dragStartPosition = ray.GetPoint(entry);

                if (_dragCoroutine != null)
                {
                    StopCoroutine(_dragCoroutine);
                }
                _dragCoroutine = StartCoroutine(Drag());
            }
        }

        private IEnumerator Drag()
        {
            while (true)
            {
                var plane = new Plane(Vector3.up, Vector3.zero);

                var ray = _camera.ScreenPointToRay(_positionAction.ReadValue<Vector2>());

                if (plane.Raycast(ray, out var entry))
                {
                    _dragCurrentPosition = ray.GetPoint(entry);

                    if (!_dragStartPosition.HasValue)
                    {
                        throw new InvalidOperationException();
                    }

                    Position = transform.position + (_dragStartPosition.Value - _dragCurrentPosition) * _dragMultiplier;
                    PositionUpdate?.Invoke(Position);
                }

                yield return null;
            }
        }

        private void DragStop(InputAction.CallbackContext context)
        {
            if (_dragCoroutine == null)
                throw new InvalidOperationException();
            StopCoroutine(_dragCoroutine);
            _dragStartPosition = null;
        }
    }
}