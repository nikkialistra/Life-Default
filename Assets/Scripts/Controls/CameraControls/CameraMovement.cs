using Colonists;
using Controls.CameraControls.Input;
using General.Map;
using General.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls.CameraControls
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CameraPositionInput))]
    [RequireComponent(typeof(CameraRotationInput))]
    [RequireComponent(typeof(CameraZoomInput))]
    [RequireComponent(typeof(CameraFocusing))]
    [RequireComponent(typeof(CameraFollowing))]
    [RequireComponent(typeof(CameraRaising))]
    [RequireComponent(typeof(CameraThresholdMovement))]
    [RequireComponent(typeof(CameraMovementApplying))]
    [RequireComponent(typeof(CameraParameters))]
    public class CameraMovement : MonoBehaviour
    {
        public Vector3 NewPosition { get; private set; }
        public Vector3 NewRotation { get; private set; }
        public float NewFieldOfView { get; private set; }

        [SerializeField] private bool _deactivateAtStartup = true;

        private Camera _camera;

        private MapInitialization _mapInitialization;
        private bool _activated;

        private CameraPositionInput _positionInput;
        private CameraRotationInput _rotationInput;
        private CameraZoomInput _zoomInput;

        private CameraFocusing _cameraFocusing;
        private CameraFollowing _cameraFollowing;

        private CameraMovementApplying _cameraMovementApplying;

        private SelectingInput _selectingInput;

        private PlayerInput _playerInput;

        private InputAction _toggleCameraMovementAction;

        [Inject]
        public void Construct(MapInitialization mapInitialization, PlayerInput playerInput)
        {
            _mapInitialization = mapInitialization;

            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _positionInput = GetComponent<CameraPositionInput>();
            _rotationInput = GetComponent<CameraRotationInput>();
            _zoomInput = GetComponent<CameraZoomInput>();

            _cameraFocusing = GetComponent<CameraFocusing>();
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraMovementApplying = GetComponent<CameraMovementApplying>();

            _toggleCameraMovementAction = _playerInput.actions.FindAction("Toggle Camera Movement");
        }

        private void OnEnable()
        {
            _toggleCameraMovementAction.started += ToggleCameraMovement;
        }

        private void OnDisable()
        {
            _toggleCameraMovementAction.started -= ToggleCameraMovement;
        }

        private void Start()
        {
            _mapInitialization.Load += OnMapInitializationLoad;

            NewPosition = transform.position;
            NewRotation = transform.rotation.eulerAngles;
            NewFieldOfView = _camera.fieldOfView;

            Activate();
        }

        private void LateUpdate()
        {
            if (!_activated) return;

            if (_cameraFollowing.TryUpdateFollow())
            {
                NewPosition = transform.position;
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
        }

        public void DeactivateMovement()
        {
            _activated = false;
            Deactivate();
        }

        public void FocusOn(Colonist colonist)
        {
            _cameraFocusing.FocusOn(colonist, NewRotation);
        }

        private void UpdateMovement()
        {
            NewPosition = _positionInput.UpdatePosition(NewPosition);
            NewRotation = _rotationInput.UpdateRotation(NewRotation);
            NewFieldOfView = _zoomInput.UpdateZoom(NewFieldOfView);

            ApplyMovement();
        }

        private void OnMapInitializationLoad()
        {
            _mapInitialization.Load -= OnMapInitializationLoad;

            _activated = Application.isEditor && _deactivateAtStartup ? false : true;

            Activate();
        }

        private void ToggleCameraMovement(InputAction.CallbackContext context)
        {
            _activated = !_activated;

            if (_activated)
                Activate();
            else
                Deactivate();
        }

        private void Activate()
        {
            _positionInput.Activate();
            _rotationInput.Activate();
            _cameraFollowing.Activate();
        }

        private void Deactivate()
        {
            _positionInput.Deactivate();
            _cameraFollowing.Deactivate();
        }

        private void UpdateFromFocusing()
        {
            NewPosition = _cameraFocusing.NewPosition;
            NewRotation = _cameraFocusing.NewRotation;
            NewFieldOfView = _cameraFocusing.NewFieldOfView;

            _cameraMovementApplying.SmoothUpdate();
        }

        private void ApplyMovement()
        {
            NewPosition = _cameraMovementApplying.ClampPositionByConstraints();

            _cameraMovementApplying.SmoothUpdate();
        }
    }
}
