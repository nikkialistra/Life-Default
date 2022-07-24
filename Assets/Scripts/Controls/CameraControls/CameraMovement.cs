using System.Collections;
using Colonists;
using General.Map;
using General.Selecting;
using Sirenix.OdinInspector;
using UI.Game;
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
    [RequireComponent(typeof(CameraMovementApplying))]
    [RequireComponent(typeof(CameraMovementParameters))]
    public class CameraMovement : MonoBehaviour
    {
        public Vector3 NewPosition => _newPosition;
        public Quaternion NewRotation => Quaternion.Euler(_newRotation);
        public float NewFieldOfView => _newFieldOfView;

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

        private float _horizontalRotation;
        private float _verticalRotation;

        private Camera _camera;

        private float _lastMousePositionX;
        private float _lastMousePositionY;

        private float _deltaMousePositionX;
        private float _deltaMousePositionY;

        private Vector3 _newPosition;
        private Vector3 _newRotation;
        private float _newFieldOfView;

        private MapInitialization _mapInitialization;
        private bool _activated;
        private bool _canMouseScroll;

        private bool _isSelectingInput;

        private GameViews _gameViews;

        private CameraFocusing _cameraFocusing;
        private CameraFollowing _cameraFollowing;
        private CameraThresholdMovement _cameraThresholdMovement;
        private CameraMovementApplying _cameraMovementApplying;
        private CameraMovementParameters _parameters;

        private SelectingInput _selectingInput;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _dragAction;
        private InputAction _mousePositionAction;
        private InputAction _zoomScrollAction;

        private InputAction _toggleCameraMovementAction;

        [Inject]
        public void Construct(MapInitialization mapInitialization, GameViews gameViews,
            SelectingInput selectingInput, PlayerInput playerInput)
        {
            _mapInitialization = mapInitialization;

            _gameViews = gameViews;
            _selectingInput = selectingInput;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _cameraFocusing = GetComponent<CameraFocusing>();
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraThresholdMovement = GetComponent<CameraThresholdMovement>();
            _cameraMovementApplying = GetComponent<CameraMovementApplying>();
            _parameters = GetComponent<CameraMovementParameters>();

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
            _newRotation = transform.rotation.eulerAngles;
            _newFieldOfView = _camera.fieldOfView;

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

        public void ActivateMovement()
        {
            _activated = true;
            Activate();
            StartCoroutine(CAllowMouseScrollALittleLater());
        }

        public void DeactivateMovement()
        {
            _activated = false;
            _canMouseScroll = false;
            Deactivate();
        }

        public void FocusOn(Colonist colonist)
        {
            _cameraFocusing.FocusOn(colonist, _newRotation);
        }

        private void UpdateMovement()
        {
            CalculateDeltas();

            UpdatePosition();
            UpdatePositionFromMouseThresholdMovement();
            UpdateRotation();
            UpdateZoom();

            ApplyMovement();
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

        private void UpdateFromFocusing()
        {
            _newPosition = _cameraFocusing.NewPosition;
            _newRotation = _cameraFocusing.NewRotation;
            _newFieldOfView = _cameraFocusing.NewFieldOfView;

            _cameraMovementApplying.SmoothUpdate();
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

        private void UpdatePositionFromMouseThresholdMovement()
        {
            if (!_canMouseScroll || !_parameters.ScreenEdgeMouseScroll || _isSelectingInput) return;

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

        private void UpdateRotation()
        {
            if (!_dragAction.IsPressed()) return;

            _verticalRotation = _newRotation.x;
            _horizontalRotation = _newRotation.y;

            _verticalRotation -= _rotateVerticalSpeed * _parameters.CameraSensitivity * _deltaMousePositionY;
            _horizontalRotation += _rotateHorizontalSpeed * _parameters.CameraSensitivity * _deltaMousePositionX;
            _newRotation = new Vector3(Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation),
                _horizontalRotation, 0.0f);
        }

        private void UpdateZoom()
        {
            if (_gameViews.MouseOverUi) return;

            var zoomScroll = _zoomScrollAction.ReadValue<Vector2>().y;
            _newFieldOfView = Mathf.Clamp(_newFieldOfView - zoomScroll * _zoomScrollSensitivity,
                _minFov, _maxFov);
        }

        private void ApplyMovement()
        {
            _newPosition = _cameraMovementApplying.ClampPositionByConstraints();

            _cameraMovementApplying.SmoothUpdate();
        }
    }
}
