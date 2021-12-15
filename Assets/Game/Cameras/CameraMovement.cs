using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        [Title("Boundaries")] 
        [SerializeField] private Vector3 _minumumPositions;
        [SerializeField] private Vector3 _maximimPositions;

        [Title("Movement")] [MinValue(0)] 
        [SerializeField]
        private float _movementNormalSpeed;

        [MinValue(0)] 
        [SerializeField] private float _movementFastSpeed;

        [MinValue(0)] 
        [SerializeField] private float _dragMultiplier;

        [Title("Rotation")] 
        [MinValue(0)] 
        [SerializeField] private float _steppedRotationAmount;

        [Title("Zoom")]
        [MinValue(0)] 
        [SerializeField] private float _buttonZoomMultiplier;

        [MinValue(0)] 
        [SerializeField] private float _scrollZoomMultiplier;

        [Title("Smooth multipliers")] 
        [MinValue(0)] 
        [SerializeField] private float _positionSmoothing;
        [MinValue(0)]
        [SerializeField] private float _rotationSmoothing;

        private Camera _camera;

        private float _movementSpeed;

        private Vector3 _newPosition;
        private Quaternion _newRotation;

        private Vector3? _dragStartPosition;
        private Vector3 _dragCurrentPosition;

        private Vector3? _rotateStartPosition;
        private Vector3 _rotateCurrentPosition;
        
        private CameraFollowing _cameraFollowing;

        private Coroutine _dragCoroutine;
        private Coroutine _moveCoroutine;
        private Coroutine _rotateCoroutine;
        private Coroutine _zoomCoroutine;

        private PlayerInput _playerInput;

        private InputAction _setFollowAction;
        private InputAction _resetFollowAction;
        private InputAction _zoomScrollAction;
        private InputAction _zoomAction;
        private InputAction _rotationAction;
        private InputAction _rotateAction;
        private InputAction _dragAction;
        private InputAction _movementAction;
        private InputAction _fastMovementAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(CameraFollowing cameraFollowing, PlayerInput playerInput)
        {
            _cameraFollowing = cameraFollowing;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            
            _setFollowAction = _playerInput.actions.FindAction("SetFollow");
            _resetFollowAction = _playerInput.actions.FindAction("ResetFollow");
            _zoomScrollAction = _playerInput.actions.FindAction("ZoomScroll");
            _zoomAction = _playerInput.actions.FindAction("Zoom");
            _rotateAction = _playerInput.actions.FindAction("Rotate");
            _dragAction = _playerInput.actions.FindAction("Drag");
            _movementAction = _playerInput.actions.FindAction("Movement");
            _fastMovementAction = _playerInput.actions.FindAction("FastMovement");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        private void OnEnable()
        {
            _setFollowAction.started += SetFollow;
            _resetFollowAction.started += ResetFollow;

            _zoomScrollAction.started += ZoomScroll;
            _zoomAction.started += ZoomStart;
            _zoomAction.canceled += ZoomStop;

            _dragAction.started += DragStart;
            _dragAction.canceled += DragStop;

            _rotateAction.started += RotateStart;
            _rotateAction.canceled += RotateStop;

            _movementAction.started += MovementStart;
            _movementAction.canceled += MovementStop;

            _fastMovementAction.started += FastMovementOn;
            _fastMovementAction.canceled += FastMovementOff;
        }

        private void OnDisable()
        {
            _setFollowAction.started -= SetFollow;
            _resetFollowAction.started -= ResetFollow;

            _zoomScrollAction.started -= ZoomScroll;
            _zoomAction.started -= ZoomStart;
            _zoomAction.canceled -= ZoomStop;

            _dragAction.started -= DragStart;
            _dragAction.canceled -= DragStop;

            _rotateAction.started -= RotateStart;
            _rotateAction.canceled -= RotateStop;

            _movementAction.started -= MovementStart;
            _movementAction.canceled -= MovementStop;

            _fastMovementAction.started -= FastMovementOn;
            _fastMovementAction.canceled -= FastMovementOff;
        }

        private void Start()
        {
            _movementSpeed = _movementNormalSpeed;

            _newPosition = transform.localPosition;
            _newRotation = transform.rotation;
        }

        private void Update()
        {
            ComputeTransform();
            if (_cameraFollowing.Following)
            {
                _newPosition += _cameraFollowing.GetDeltaFollowPosition();
            }
        }
        
        private void SetFollow(InputAction.CallbackContext context)
        {
            var screenPoint = _positionAction.ReadValue<Vector2>();
            if (!_cameraFollowing.TryFollow(screenPoint))
            {
                _cameraFollowing.Reset();
            }
        }

        private void ResetFollow(InputAction.CallbackContext context)
        {
            _cameraFollowing.Reset();
        }

        private void ZoomScroll(InputAction.CallbackContext context)
        {
            var zoomPosition = _newPosition;
            var zooming = context.ReadValue<Vector2>().y;
            if (zooming == 0)
            {
                return;
            }

            var localZoomAmount = transform.forward * zooming;
            zoomPosition += localZoomAmount * _scrollZoomMultiplier ;
            ClampZoomByConstraints(zoomPosition);
        }

        private void ZoomStart(InputAction.CallbackContext context)
        {
            if (_zoomCoroutine != null)
                StopCoroutine(_zoomCoroutine);
            _zoomCoroutine = StartCoroutine(Zoom());
        }

        private IEnumerator Zoom()
        {
            while (true)
            {
                var zooming = _zoomAction.ReadValue<float>();
                if (zooming != 0)
                {
                    UpdateZoom(zooming);
                }

                yield return null;
            }
        }

        private void UpdateZoom(float zooming)
        {
            var zoomPosition = _newPosition;
            var localZoomAmount = transform.forward * zooming;
            zoomPosition += localZoomAmount * _buttonZoomMultiplier * Time.deltaTime;
            
            ClampZoomByConstraints(zoomPosition);
        }

        private void ClampZoomByConstraints(Vector3 zoomPosition)
        {
            if (zoomPosition.y < _minumumPositions.y)
            {
                zoomPosition = _newPosition;
                zoomPosition.y = _minumumPositions.y;
            }

            if (zoomPosition.y > _maximimPositions.y)
            {
                zoomPosition = _newPosition;
                zoomPosition.y = _maximimPositions.y;
            }

            _newPosition = zoomPosition;
        }

        private void ZoomStop(InputAction.CallbackContext context)
        {
            if (_zoomCoroutine == null)
                throw new InvalidOperationException();
            StopCoroutine(_zoomCoroutine);
        }

        private void DragStart(InputAction.CallbackContext context)
        {
            var plane = new Plane(Vector3.up, Vector3.zero);

            var ray = _camera.ScreenPointToRay(_positionAction.ReadValue<Vector2>());

            if (plane.Raycast(ray, out var entry))
            {
                _dragStartPosition = ray.GetPoint(entry);

                if (_dragCoroutine != null)
                    StopCoroutine(_dragCoroutine);
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
                        throw new InvalidOperationException();

                    _newPosition = transform.position +
                                   (_dragStartPosition.Value - _dragCurrentPosition) * _dragMultiplier;
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

        private void RotateStart(InputAction.CallbackContext context)
        {
            if (_rotateCoroutine != null)
                StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            while (true)
            {
                var rotation = _rotateAction.ReadValue<float>();
                UpdateNewRotation(rotation * _steppedRotationAmount * Time.deltaTime);

                yield return null;
            }
        }

        private void UpdateNewRotation(float amount)
        {
            var newRotationEulerAngles = _newRotation.eulerAngles;
            newRotationEulerAngles.y += amount;
            _newRotation = Quaternion.Euler(newRotationEulerAngles);
        }

        private void RotateStop(InputAction.CallbackContext context)
        {
            if (_rotateCoroutine == null)
                throw new InvalidOperationException();
            StopCoroutine(_rotateCoroutine);
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
            var position = _newPosition;
            position += transform.right * movement.x;
            position += transform.up * movement.y + transform.forward * movement.y;

            ClampPositionByConstraints(position);
        }

        private void ClampPositionByConstraints(Vector3 position)
        {
            _newPosition = position;
            
            if (position.x < _minumumPositions.x)
            {
                _newPosition.x = _minumumPositions.x;
            }
            
            if (position.x > _maximimPositions.x)
            {
                _newPosition.x = _maximimPositions.x;
            }
            
            if (position.z < _minumumPositions.z)
            {
                _newPosition.z = _minumumPositions.z;
            }
            
            if (position.z > _maximimPositions.z)
            {
                _newPosition.z = _maximimPositions.z;
            }
        }

        private void MovementStop(InputAction.CallbackContext context)
        {
            if (_moveCoroutine == null)
                throw new InvalidOperationException();
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


        private void ComputeTransform()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition, _positionSmoothing * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationSmoothing * Time.deltaTime);
        }
    }
}