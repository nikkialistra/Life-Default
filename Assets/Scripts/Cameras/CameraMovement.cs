using MapGeneration.Map;
using Saving;
using Sirenix.OdinInspector;
using Units.Services.Selecting;
using Units.Unit;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
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
        [SerializeField] private float _zoomSmoothing;

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
        private float _newFieldOfView;

        private Map _map;
        private bool _deactivated;

        private UnitChoosing _unitChoosing;
        private UnitFacade _unit;
        private Transform _followTransform;
        private Vector3 _offset;
        private bool _following;

        private float _cameraSensitivity;
        private bool _screenEdgeMouseScroll;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _dragAction;
        private InputAction _mousePositionAction;
        private InputAction _zoomScrollAction;
        private InputAction _zoomButtonAction;

        private InputAction _setFollowAction;

        private InputAction _toggleCameraMovementAction;


        [Inject]
        public void Construct(bool isSetUpSession, Map map, UnitChoosing unitChoosing, PlayerInput playerInput)
        {
            if (isSetUpSession)
            {
                _deactivated = true;
            }

            _unitChoosing = unitChoosing;

            _map = map;

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

            _setFollowAction = _playerInput.actions.FindAction("Set Follow");

            _toggleCameraMovementAction = _playerInput.actions.FindAction("Toggle Camera Movement");
        }

        private void OnEnable()
        {
            _toggleCameraMovementAction.started += ToggleCameraMovementMovement;
        }

        private void OnDisable()
        {
            _toggleCameraMovementAction.started -= ToggleCameraMovementMovement;
        }

        private void Start()
        {
            _map.Load += OnMapLoad;

            _yPosition = transform.position.y;

            _newPosition = transform.position;
            _newRotation = transform.rotation;
            _newFieldOfView = _camera.fieldOfView;

            LoadSettings();
            Activate();
        }

        private void LateUpdate()
        {
            if (_deactivated)
            {
                return;
            }

            LoadSettings();
            CalculateDeltas();

            UpdateFollowing();
            if (_following)
            {
                UpdateFollow();
            }
            else
            {
                UpdatePosition();
                UpdatePositionFromMouseThresholdMovement();
                UpdateRotation();
            }

            UpdateZoom();

            ClampPositionByConstraints();
            SmoothUpdate();
        }

        private void OnMapLoad()
        {
            _map.Load -= OnMapLoad;

            _deactivated = false;
        }

        private void ToggleCameraMovementMovement(InputAction.CallbackContext context)
        {
            _deactivated = !_deactivated;

            if (_deactivated)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }

        private void LoadSettings()
        {
            _cameraSensitivity = GameSettings.Instance.CameraSensitivity;
            _screenEdgeMouseScroll = GameSettings.Instance.ScreenEdgeMouseScroll;
        }

        private void Deactivate()
        {
            _setFollowAction.started -= TryFollow;
            _unitChoosing.UnitChosen -= SetFollow;
        }

        private void Activate()
        {
            _setFollowAction.started += TryFollow;
            _unitChoosing.UnitChosen += SetFollow;

            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }

        private void UpdateFollowing()
        {
            var keyboardMoved = _movementAction.ReadValue<Vector2>() != Vector2.zero;

            var position = _mousePositionAction.ReadValue<Vector2>();
            var normalisedPosition = GetNormalisedPosition(position);
            var mouseMoved = Mathf.Abs(normalisedPosition.x) > _positionMoveXThreshold ||
                             Mathf.Abs(normalisedPosition.y) > _positionMoveYThreshold;

            if (keyboardMoved || mouseMoved || _dragAction.IsPressed())
            {
                _following = false;
            }
        }

        private void UpdateFollow()
        {
            transform.position = _followTransform.position + _offset;
            _newPosition = transform.position;
        }

        private void TryFollow(InputAction.CallbackContext context)
        {
            var screenPoint = _mousePositionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<UnitFacade>(out var unit))
                {
                    SetFollow(unit);
                }
                else
                {
                    ResetFollow();
                }
            }
        }

        private void SetFollow(UnitFacade unit)
        {
            UnsubscribeFromLastUnit();

            _unit = unit;
            _followTransform = unit.transform;
            _offset = _newPosition - _followTransform.position;

            unit.Die += ResetFollow;

            _following = true;
        }

        private void ResetFollow()
        {
            _followTransform = null;
            _following = false;
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_unit != null)
            {
                _unit.Die -= ResetFollow;
            }
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

            _newPosition += transform.right * (_moveSpeed * Time.deltaTime * movement.x);
            _newPosition += transform.forward * (_moveSpeed * Time.deltaTime * movement.y);
        }

        private void UpdateRotation()
        {
            if (!_dragAction.IsPressed())
            {
                return;
            }

            _verticalRotation = _newRotation.eulerAngles.x;
            _horizontalRotation = _newRotation.eulerAngles.y;

            _verticalRotation -= _rotateVerticalSpeed * _cameraSensitivity * _deltaMousePositionY;
            _horizontalRotation += _rotateHorizontalSpeed * _cameraSensitivity * _deltaMousePositionX;
            _newRotation.eulerAngles =
                new Vector3(Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation),
                    _horizontalRotation, 0.0f);
        }

        private void UpdateZoom()
        {
            var zoomScroll = _zoomScrollAction.ReadValue<Vector2>().y;
            _newFieldOfView = Mathf.Clamp(_newFieldOfView - zoomScroll * _zoomScrollSensitivity,
                _minFov, _maxFov);

            var zoomButton = _zoomButtonAction.ReadValue<float>();
            _newFieldOfView = Mathf.Clamp(_newFieldOfView - zoomButton * _zoomButtonSensitivity,
                _minFov, _maxFov);
        }

        private void UpdatePositionFromMouseThresholdMovement()
        {
            if (!_screenEdgeMouseScroll)
            {
                return;
            }

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
                UpdatePositionFromMouseMovement(movement);
            }
        }

        //Result between -1 and 1 is the result between 0 and 1 subtracted with 0.5 and multiplied by 2
        private Vector2 GetNormalisedPosition(Vector2 position)
        {
            var result = new Vector2(((position.x / Screen.width) - 0.5f) * 2f,
                ((position.y / Screen.height) - 0.5f) * 2f);
            return result;
        }

        private void UpdatePositionFromMouseMovement(Vector2 movement)
        {
            _newPosition += transform.right * movement.x;
            _newPosition += transform.up * movement.y + transform.forward * movement.y;
        }

        private void ClampPositionByConstraints()
        {
            var position = _newPosition;

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

            _newPosition = position;
        }

        private void SmoothUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition, _positionSmoothing * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationSmoothing * Time.deltaTime);
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _newFieldOfView, _zoomSmoothing * Time.deltaTime);
        }
    }
}
