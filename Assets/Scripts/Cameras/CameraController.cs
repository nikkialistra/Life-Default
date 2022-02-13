using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Title("Movement")]
        [SerializeField] private float _moveSpeed;

        [Space]
        [Range(0.5f, 1f)]
        [SerializeField] private float _positionMoveXThreshold;
        [Range(0.5f, 1f)]
        [SerializeField] private float _positionMoveYThreshold;

        [Title("Rotation")]
        [SerializeField] private float _rotateHorizontalSpeed;
        [SerializeField] private float _rotateVerticalSpeed;

        [SerializeField] private float _minVerticalRotation;
        [SerializeField] private float _maxVerticalRotation;

        [Title("Zoom")]
        [SerializeField] private float _zoomScrollSensitivity;
        [SerializeField] private float _zoomButtonSensitivity;

        [SerializeField] private float _minFov;
        [SerializeField] private float _maxFov;

        [Title("Boundaries")]
        [ValidateInput("@_minimumPositionX < _maximumPositionX",
            "Minimum position should be less than maximum position")]
        [SerializeField] private float _minimumPositionX;

        [ValidateInput("@_minimumPositionX < _maximumPositionX",
            "Minimum position should be less than maximum position")]
        [SerializeField] private float _maximumPositionX;

        [Space]
        [ValidateInput("@_minimumPositionZ < _maximumPositionZ",
            "Minimum position should be less than maximum position")]
        [SerializeField] private float _minimumPositionZ;

        [ValidateInput("@_minimumPositionZ < _maximumPositionZ",
            "Minimum position should be less than maximum position")]
        [SerializeField] private float _maximumPositionZ;

        [Title("Smoothing")]
        [SerializeField] private float _positionSmoothing;
        [SerializeField] private float _rotationSmoothing;

        private float _horizontalRotation;
        private float _verticalRotation;
        private float _yPosition;

        private Camera _camera;

        private float _lastMousePositionX;
        private float _lastMousePositionY;

        private float _deltaMousePositionX;
        private float _deltaMousePositionY;

        private Vector3 _newPosition;
        private Quaternion _newRotation;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _dragAction;
        private InputAction _mousePositionAction;
        private InputAction _zoomScrollAction;
        private InputAction _zoomButtonAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _movementAction = _playerInput.actions.FindAction("Movement");
            _dragAction = _playerInput.actions.FindAction("Drag");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _zoomScrollAction = _playerInput.actions.FindAction("Zoom Scroll");
            _zoomButtonAction = _playerInput.actions.FindAction("Zoom Button");
        }

        private void Start()
        {
            _yPosition = transform.position.y;
        }

        private void LateUpdate()
        {
            _newPosition = transform.position;
            _newRotation = transform.rotation;

            CalculateDeltas();

            UpdatePosition();
            UpdatePositionFromMouseThresholdMovement();
            UpdateRotation();
            UpdateZoom();

            ClampPositionByConstraints();
        }

        private void CalculateDeltas()
        {
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _deltaMousePositionX = mousePosition.x - _lastMousePositionX;
            _deltaMousePositionY = mousePosition.y - _lastMousePositionY;

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }

        private void UpdatePosition()
        {
            var movement = _movementAction.ReadValue<Vector2>();

            transform.position += transform.right * (_moveSpeed * Time.deltaTime * movement.x);
            transform.position += transform.forward * (_moveSpeed * Time.deltaTime * movement.y);
        }

        private void UpdateRotation()
        {
            if (!_dragAction.IsPressed())
            {
                return;
            }

            _verticalRotation = transform.eulerAngles.x;
            _horizontalRotation = transform.eulerAngles.y;

            _verticalRotation -= _rotateVerticalSpeed * _deltaMousePositionY;
            _horizontalRotation += _rotateHorizontalSpeed * _deltaMousePositionX;
            transform.eulerAngles =
                new Vector3(Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation),
                    _horizontalRotation, 0.0f);
        }

        private void UpdateZoom()
        {
            var zoomScroll = _zoomScrollAction.ReadValue<Vector2>().y;
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - zoomScroll * _zoomScrollSensitivity,
                _minFov, _maxFov);

            var zoomButton = _zoomButtonAction.ReadValue<float>();
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - zoomButton * _zoomButtonSensitivity,
                _minFov, _maxFov);
        }

        private void UpdatePositionFromMouseThresholdMovement()
        {
            var position = _mousePositionAction.ReadValue<Vector2>();
            var normalisedPosition = GetNormalisedPosition(position);
            var movement = Vector2.zero;

            if (Mathf.Abs(normalisedPosition.x) > _positionMoveXThreshold)
            {
                movement.x = Mathf.Sign(normalisedPosition.x) * _moveSpeed * Time.deltaTime;
            }

            if (Mathf.Abs(normalisedPosition.y) > _positionMoveYThreshold)
            {
                movement.y = Mathf.Sign(normalisedPosition.y) * _moveSpeed * Time.deltaTime;
            }

            if (movement != Vector2.zero)
            {
                UpdatePosition(movement);
            }
        }

        //Result between -1 and 1 is the result between 0 and 1 subtracted with 0.5 and multiplied by 2
        private Vector2 GetNormalisedPosition(Vector2 position)
        {
            var result = new Vector2(((position.x / Screen.width) - 0.5f) * 2f,
                ((position.y / Screen.height) - 0.5f) * 2f);
            return result;
        }

        private void UpdatePosition(Vector2 movement)
        {
            transform.position += transform.right * movement.x;
            transform.position += transform.up * movement.y + transform.forward * movement.y;
        }

        private void ClampPositionByConstraints()
        {
            var position = transform.position;

            position.y = _yPosition;

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

            transform.position = position;
        }

        private void SmoothUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition, _positionSmoothing * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationSmoothing * Time.deltaTime);
        }
    }
}
