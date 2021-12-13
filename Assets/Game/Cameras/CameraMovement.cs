using System;
using System.Collections;
using Kernel.Types;
using Selecting.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private float _movementNormalSpeed;
        [SerializeField] private float _movementFastSpeed;
        [SerializeField] private float _dragMultiplier;

        [Header("Rotation")] 
        [SerializeField] private float _steppedRotationAmount;
        [SerializeField] private float _touchRotationMultiplier;

        [Header("Zoom")] 
        [SerializeField] private Vector3 _zoomAmount;
        [SerializeField] private float _zoomMultiplier;

        [Header("Multipliers")] 
        [SerializeField] private float _movementTime;

        [SerializeField] private float _rotationTime;
        [SerializeField] private float _zoomTime;

        private Camera _camera;
        private Transform _cameraTransform;

        private Control _control;

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

        private void Awake()
        {
            _control = new Control();
        }

        private void OnEnable()
        {
            _control.Enable();
            _control.Camera.SetFollow.started += SetFollow;
            _control.Camera.ResetFollow.started += ResetFollow;
            _control.Camera.Scroll.started += Scroll;
            _control.Camera.Drag.started += DragStart;
            _control.Camera.Drag.canceled += DragStop;
            _control.Camera.Rotation.started += RotationStart;
            _control.Camera.Rotation.canceled += RotationEnd;
            _control.Camera.FastMovement.started += FastMovementOn;
            _control.Camera.FastMovement.canceled += FastMovementOff;
            _control.Camera.Movement.started += MovementStart;
            _control.Camera.Movement.canceled += MovementStop;
            _control.Camera.Rotate.started += RotateStart;
            _control.Camera.Rotate.canceled += RotateStop;
            _control.Camera.Zoom.started += ZoomStart;
            _control.Camera.Zoom.canceled += ZoomStop;
        }

        private void OnDisable()
        {
            _control.Camera.SetFollow.started -= SetFollow;
            _control.Camera.ResetFollow.started -= ResetFollow;
            _control.Camera.Scroll.started -= Scroll;
            _control.Camera.Drag.started -= DragStart;
            _control.Camera.Drag.canceled -= DragStop;
            _control.Camera.Rotation.started -= RotationStart;
            _control.Camera.Rotation.canceled -= RotationEnd;
            _control.Camera.FastMovement.started -= FastMovementOn;
            _control.Camera.FastMovement.canceled -= FastMovementOff;
            _control.Camera.Movement.started -= MovementStart;
            _control.Camera.Movement.canceled -= MovementStop;
            _control.Camera.Rotate.started -= RotateStart;
            _control.Camera.Rotate.canceled -= RotateStop;
            _control.Camera.Zoom.started -= ZoomStart;
            _control.Camera.Zoom.canceled -= ZoomStop;
            _control.Disable();
        }
        
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _cameraTransform = _camera.transform;

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
            var ray = _camera.ScreenPointToRay(_control.Camera.Position.ReadValue<Vector2>());

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.GetComponent<ISelectable>() != null)
                {
                    _followTransform = hit.transform;
                }
            }
        }

        private void ResetFollow(InputAction.CallbackContext context) => _followTransform = null;

        private void Scroll(InputAction.CallbackContext context) => _newZoom += _zoomAmount * (context.ReadValue<Vector2>().y * _zoomMultiplier);

        private void DragStart(InputAction.CallbackContext context)
        {
            var plane = new Plane(Vector3.up, Vector3.zero);

            var ray = _camera.ScreenPointToRay(_control.Camera.Position.ReadValue<Vector2>());

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

                var ray = _camera.ScreenPointToRay(_control.Camera.Position.ReadValue<Vector2>());

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
            _rotateStartPosition = _control.Camera.Position.ReadValue<Vector2>();
        }

        private void RotationEnd(InputAction.CallbackContext context)
        {
            _rotateCurrentPosition = _control.Camera.Position.ReadValue<Vector2>();

            if (!_rotateStartPosition.HasValue)
                throw new InvalidOperationException();

            var difference = _rotateCurrentPosition - _rotateStartPosition.Value;

            _rotateStartPosition = _rotateCurrentPosition;

            _newRotation *= Quaternion.Euler(Vector3.up * (difference.x * -_touchRotationMultiplier));
        }

        private void FastMovementOn(InputAction.CallbackContext context) => _movementSpeed = _movementFastSpeed;

        private void FastMovementOff(InputAction.CallbackContext context) => _movementSpeed = _movementNormalSpeed;

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
                var movement = _control.Camera.Movement.ReadValue<Vector2>() * (_movementSpeed * Time.deltaTime);
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
                var rotation = _control.Camera.Rotate.ReadValue<float>();
                _newRotation *= Quaternion.Euler(Vector3.up * (rotation * _steppedRotationAmount * Time.deltaTime));

                yield return null;
            }
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
                if (_control.Camera.Zoom.ReadValue<float>() > 0)
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