using System;
using System.Collections;
using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        [Title("Movement")] 
        [MinValue(0)]
        [SerializeField] private float _movementNormalSpeed;
        [MinValue(0)]
        [SerializeField] private float _movementFastSpeed;
        [MinValue(0)]
        [SerializeField] private float _dragMultiplier;

        [Title("Rotation")] 
        [MinValue(0)]
        [SerializeField] private float _steppedRotationAmount;
        [MinValue(0)]
        [SerializeField] private float _touchRotationMultiplier;

        [Title("Zoom")] 
        [SerializeField] private Vector3 _zoomAmount;
        [MinValue(0)]
        [SerializeField] private float _zoomMultiplier;

        [Title("Smooth multipliers")] 
        [MinValue(0)]
        [SerializeField] private float _movementTime;
        [MinValue(0)]
        [SerializeField] private float _rotationTime;
        [MinValue(0)]
        [SerializeField] private float _zoomTime;

        private Camera _camera;
        private Transform _cameraTransform;

        private PlayerInput _playerInput;

        private float _movementSpeed;

        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private Vector3 _newZoom;

        private Vector3? _dragStartPosition;
        private Vector3 _dragCurrentPosition;

        private Vector3? _rotateStartPosition;
        private Vector3 _rotateCurrentPosition;

        private Transform _followTransform;

        private Coroutine _dragCoroutine;
        private Coroutine _moveCoroutine;
        private Coroutine _rotateCoroutine;
        private Coroutine _zoomCoroutine;
        
        private InputAction _setFollowAction;
        private InputAction _resetFollowAction;
        private InputAction _scrollAction;
        private InputAction _dragAction;
        private InputAction _rotationAction;
        private InputAction _movementAction;
        private InputAction _fastMovementAction;
        private InputAction _rotateAction;
        private InputAction _zoomAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(Camera camera, PlayerInput playerInput)
        {
            _camera = camera;
            _cameraTransform = camera.transform;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _setFollowAction = _playerInput.actions.FindAction("SetFollow");
            _resetFollowAction = _playerInput.actions.FindAction("ResetFollow");
            _scrollAction = _playerInput.actions.FindAction("Scroll");
            _dragAction = _playerInput.actions.FindAction("Drag");
            _rotationAction = _playerInput.actions.FindAction("Rotation");
            _movementAction = _playerInput.actions.FindAction("Movement");
            _fastMovementAction = _playerInput.actions.FindAction("FastMovement");
            _rotateAction = _playerInput.actions.FindAction("Rotate");
            _zoomAction = _playerInput.actions.FindAction("Zoom");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        private void OnEnable()
        {
            _setFollowAction.started += SetFollow;
            _resetFollowAction.started += ResetFollow;
            
            _scrollAction.started += Scroll;

            _dragAction.started += DragStart;
            _dragAction.canceled += DragStop;

            _rotationAction.started += RotationStart;
            _rotationAction.canceled += RotationEnd;

            _movementAction.started += MovementStart;
            _movementAction.canceled += MovementStop;

            _fastMovementAction.started += FastMovementOn;
            _fastMovementAction.canceled += FastMovementOff;

            _rotateAction.started += RotateStart;
            _rotateAction.canceled += RotateStop;

            _zoomAction.started += ZoomStart;
            _zoomAction.canceled += ZoomStop;
        }
        
        private void OnDisable ()
        {
            _setFollowAction.started -= SetFollow;
            _resetFollowAction.started -= ResetFollow;
            
            _scrollAction.started -= Scroll;

            _dragAction.started -= DragStart;
            _dragAction.canceled -= DragStop;

            _rotationAction.started -= RotationStart;
            _rotationAction.canceled -= RotationEnd;

            _movementAction.started -= MovementStart;
            _movementAction.canceled -= MovementStop;

            _fastMovementAction.started -= FastMovementOn;
            _fastMovementAction.canceled -= FastMovementOff;

            _rotateAction.started -= RotateStart;
            _rotateAction.canceled -= RotateStop;

            _zoomAction.started -= ZoomStart;
            _zoomAction.canceled -= ZoomStop;
        }

        private void Start()
        {
            _movementSpeed = _movementNormalSpeed;

            _newPosition = transform.position;
            _newRotation = transform.rotation;
            _newZoom = _cameraTransform.localPosition;
        }

        private void Update()
        {
            if (_followTransform != null)
                _newPosition = _followTransform.position;
            ComputeTransform();
        }

        private void SetFollow(InputAction.CallbackContext context)
        {
            var ray = _camera.ScreenPointToRay(_positionAction.ReadValue<Vector2>());

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.GetComponent<ISelectable>() != null)
                {
                    _followTransform = hit.transform;
                }
            }
        }

        private void ResetFollow(InputAction.CallbackContext context)
        {
            _followTransform = null;
        }

        private void Scroll(InputAction.CallbackContext context)
        {
            _newZoom += _zoomAmount * (context.ReadValue<Vector2>().y * _zoomMultiplier);
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

        private void RotationStart(InputAction.CallbackContext context)
        {
            _rotateStartPosition = _positionAction.ReadValue<Vector2>();
        }

        private void RotationEnd(InputAction.CallbackContext context)
        {
            _rotateCurrentPosition = _positionAction.ReadValue<Vector2>();

            if (!_rotateStartPosition.HasValue)
                throw new InvalidOperationException();

            var difference = _rotateCurrentPosition - _rotateStartPosition.Value;

            _rotateStartPosition = _rotateCurrentPosition;
            
            UpdateNewRotation(difference.x * -_touchRotationMultiplier);
        }

        private void FastMovementOn(InputAction.CallbackContext context)
        {
            _movementSpeed = _movementFastSpeed;
        }

        private void FastMovementOff(InputAction.CallbackContext context)
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
                var movement = _movementAction.ReadValue<Vector2>() * (_movementSpeed * Time.deltaTime);
                _newPosition += new Vector3(movement.x, 0, movement.y);

                yield return null;
            }
        }

        private void MovementStop(InputAction.CallbackContext context)
        {
            if (_moveCoroutine == null)
                throw new InvalidOperationException();
            StopCoroutine(_moveCoroutine);
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
                if (_zoomAction.ReadValue<float>() > 0)
                    _newZoom += _zoomAmount * (_zoomMultiplier * Time.deltaTime);
                else
                    _newZoom -= _zoomAmount * (_zoomMultiplier * Time.deltaTime);

                yield return null;
            }
        }

        private void ZoomStop(InputAction.CallbackContext context)
        {
            if (_zoomCoroutine == null)
                throw new InvalidOperationException();
            StopCoroutine(_zoomCoroutine);
        }

        private void ComputeTransform()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition, _movementTime * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationTime * Time.deltaTime);
            _cameraTransform.localPosition =
                Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _zoomTime * Time.deltaTime);
        }
    }
}