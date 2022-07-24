using System.Collections;
using Colonists;
using General.Map;
using General.Selecting;
using Saving;
using Sirenix.OdinInspector;
using UI.Game;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls.CameraControls
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CameraFocusing))]
    [RequireComponent(typeof(CameraFollowing))]
    [RequireComponent(typeof(CameraRaising))]
    [RequireComponent(typeof(CameraThresholdMovement))]
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private bool _deactivateAtStartup = true;

        [Title("Movement")]
        [SerializeField] private float _moveSpeed;

        [Title("Rotation")]
        [SerializeField] private float _rotateHorizontalSpeed;
        [SerializeField] private float _rotateVerticalSpeed;

        [SerializeField] private float _minVerticalRotation;
        [SerializeField] private float _maxVerticalRotation;

        [Title("Zoom")]
        [SerializeField] private float _zoomScrollSensitivity;

        [SerializeField] private float _minFov;
        [SerializeField] private float _maxFov;

        [Title("Boundaries")]
        [SerializeField] private float _minimumPositionX;
        [SerializeField] private float _maximumPositionX;

        [Space]
        [SerializeField] private float _minimumPositionZ;
        [SerializeField] private float _maximumPositionZ;

        [Title("Smoothing")]
        [SerializeField] private float _positionSmoothing;
        [SerializeField] private float _rotationSmoothing;
        [SerializeField] private float _zoomSmoothing;

        private float _horizontalRotation;
        private float _verticalRotation;

        private Camera _camera;

        private float _lastMousePositionX;
        private float _lastMousePositionY;

        private float _deltaMousePositionX;
        private float _deltaMousePositionY;

        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private float _newFieldOfView;

        private MapInitialization _mapInitialization;
        private bool _activated;
        private bool _canMouseScroll;

        private float _cameraSensitivity;
        private bool _screenEdgeMouseScroll;
        private bool _isSelectingInput;

        private float _raiseDistance;

        private GameSettings _gameSettings;
        private GameViews _gameViews;

        private CameraFocusing _cameraFocusing;
        private CameraFollowing _cameraFollowing;
        private CameraRaising _cameraRaising;
        private CameraThresholdMovement _cameraThresholdMovement;

        private SelectingInput _selectingInput;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _dragAction;
        private InputAction _mousePositionAction;
        private InputAction _zoomScrollAction;

        private InputAction _toggleCameraMovementAction;

        [Inject]
        public void Construct(MapInitialization mapInitialization, GameSettings gameSettings, GameViews gameViews,
            SelectingInput selectingInput, PlayerInput playerInput)
        {
            _mapInitialization = mapInitialization;

            _gameSettings = gameSettings;
            _gameViews = gameViews;
            _selectingInput = selectingInput;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _cameraFocusing = GetComponent<CameraFocusing>();
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraRaising = GetComponent<CameraRaising>();
            _cameraThresholdMovement = GetComponent<CameraThresholdMovement>();

            _movementAction = _playerInput.actions.FindAction("Movement");
            _dragAction = _playerInput.actions.FindAction("Drag");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _zoomScrollAction = _playerInput.actions.FindAction("Zoom Scroll");

            _toggleCameraMovementAction = _playerInput.actions.FindAction("Toggle Camera Movement");
        }

        private void OnEnable()
        {
            _toggleCameraMovementAction.started += ToggleCameraMovement;

            _selectingInput.Selecting += OnSelecting;
            _selectingInput.SelectingEnd += OnSelectingEnd;
        }

        private void OnDisable()
        {
            _toggleCameraMovementAction.started -= ToggleCameraMovement;

            _selectingInput.Selecting -= OnSelecting;
            _selectingInput.SelectingEnd -= OnSelectingEnd;
        }

        private void Start()
        {
            _mapInitialization.Load += OnMapInitializationLoad;

            _newPosition = transform.position;
            _newRotation = transform.rotation;
            _newFieldOfView = _camera.fieldOfView;

            LoadSettings();
            Activate();
        }

        private void LateUpdate()
        {
            if (!_activated) return;

            if (_cameraFollowing.TryUpdateFollow())
            {
                _newPosition = transform.position;
                return;
            }

            if (_cameraFocusing.Focusing)
            {
                UpdateFromFocusing();
                return;
            }

            UpdateMovement();
        }

        public void DeactivateMovement()
        {
            _activated = false;
            _canMouseScroll = false;
            Deactivate();
        }

        public void ActivateMovement()
        {
            _activated = true;
            Activate();
            StartCoroutine(CAllowMouseScrollALittleLater());
        }

        public void FocusOn(Colonist colonist)
        {
            _cameraFocusing.FocusOn(colonist, _newRotation);
        }

        private void UpdateMovement()
        {
            CalculateDeltas();

            {
                UpdatePosition();
                UpdatePositionFromMouseThresholdMovement();
                UpdateRotation();
            }

            UpdateZoom();

            ClampPositionByConstraints();

            SmoothUpdate();
        }

        private void OnMapInitializationLoad()
        {
            _mapInitialization.Load -= OnMapInitializationLoad;

            _activated = Application.isEditor && _deactivateAtStartup ? false : true;

            StartCoroutine(CAllowMouseScrollALittleLater());
        }

        private IEnumerator CAllowMouseScrollALittleLater()
        {
            yield return new WaitForSeconds(0.1f);
            _canMouseScroll = true;
        }

        private void ToggleCameraMovement(InputAction.CallbackContext context)
        {
            _activated = !_activated;

            if (_activated)
                Activate();
            else
                Deactivate();
        }

        private void OnSelecting(Rect _)
        {
            _isSelectingInput = true;
        }

        private void OnSelectingEnd(Rect _)
        {
            _isSelectingInput = false;
        }

        private void LoadSettings()
        {
            _cameraSensitivity = _gameSettings.CameraSensitivity.Value;
            _screenEdgeMouseScroll = _gameSettings.ScreenEdgeMouseScroll.Value;

            _gameSettings.CameraSensitivity.Subscribe(value => _cameraSensitivity = value);
            _gameSettings.ScreenEdgeMouseScroll.Subscribe(value => _screenEdgeMouseScroll = value);
        }

        private void Activate()
        {
            _cameraFollowing.Activate();

            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }

        private void Deactivate()
        {
            _cameraFollowing.Deactivate();
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

            _newPosition += transform.right * (_moveSpeed * Time.unscaledDeltaTime * movement.x);
            _newPosition += transform.forward * (_moveSpeed * Time.unscaledDeltaTime * movement.y);
        }

        private void UpdateRotation()
        {
            if (!_dragAction.IsPressed()) return;

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
            if (_gameViews.MouseOverUi) return;

            var zoomScroll = _zoomScrollAction.ReadValue<Vector2>().y;
            _newFieldOfView = Mathf.Clamp(_newFieldOfView - zoomScroll * _zoomScrollSensitivity,
                _minFov, _maxFov);
        }

        private void UpdatePositionFromMouseThresholdMovement()
        {
            if (!_canMouseScroll || !_screenEdgeMouseScroll || _isSelectingInput) return;

            var position = _mousePositionAction.ReadValue<Vector2>();

            var movement = _cameraThresholdMovement.GetMovementFromPosition(position, _moveSpeed);

            if (movement != Vector2.zero)
                UpdatePositionFromMouseMovement(movement);
        }

        private void UpdatePositionFromMouseMovement(Vector2 movement)
        {
            _newPosition += transform.right * movement.x;
            _newPosition += transform.up * movement.y + transform.forward * movement.y;
        }

        private void ClampPositionByConstraints()
        {
            var position = _newPosition;

            position = _cameraRaising.RaiseAboveTerrain(position);

            if (position.x < _minimumPositionX)
                position.x = _minimumPositionX;

            if (position.x > _maximumPositionX)
                position.x = _maximumPositionX;

            if (position.z < _minimumPositionZ)
                position.z = _minimumPositionZ;

            if (position.z > _maximumPositionZ)
                position.z = _maximumPositionZ;

            _newPosition = position;
        }

        private void UpdateFromFocusing()
        {
            _newPosition = _cameraFocusing.NewPosition;
            _newRotation.eulerAngles = _cameraFocusing.NewRotation;
            _newFieldOfView = _cameraFocusing.NewFieldOfView;

            SmoothUpdate();
        }

        private void SmoothUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition,
                _positionSmoothing * Time.unscaledDeltaTime) + new Vector3(0, _raiseDistance, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation,
                _rotationSmoothing * Time.unscaledDeltaTime);

            _camera.fieldOfView =
                Mathf.Lerp(_camera.fieldOfView, _newFieldOfView, _zoomSmoothing * Time.unscaledDeltaTime);
        }
    }
}
