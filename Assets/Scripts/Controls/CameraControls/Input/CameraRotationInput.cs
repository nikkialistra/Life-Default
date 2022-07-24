using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls.CameraControls.Input
{
    [RequireComponent(typeof(CameraParameters))]
    public class CameraRotationInput : MonoBehaviour
    {
        [SerializeField] private float _rotateHorizontalSpeed;
        [SerializeField] private float _rotateVerticalSpeed;

        [SerializeField] private float _minVerticalRotation;
        [SerializeField] private float _maxVerticalRotation;

        private float _horizontalRotation;
        private float _verticalRotation;

        private float _lastMousePositionX;
        private float _lastMousePositionY;

        private float _deltaMousePositionX;
        private float _deltaMousePositionY;

        private CameraParameters _parameters;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;
        private InputAction _dragAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _parameters = GetComponent<CameraParameters>();

            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _dragAction = _playerInput.actions.FindAction("Drag");
        }

        public void Activate()
        {
            ResetDeltas();
        }

        public Vector3 UpdateRotation(Vector3 rotation)
        {
            if (!_dragAction.IsPressed())
            {
                ResetDeltas();
                return rotation;
            }

            CalculateDeltas();

            return DeltaRotationFromMouseMove(rotation);
        }

        private void CalculateDeltas()
        {
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _deltaMousePositionX = mousePosition.x - _lastMousePositionX;
            _deltaMousePositionY = mousePosition.y - _lastMousePositionY;

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }

        private Vector3 DeltaRotationFromMouseMove(Vector3 rotation)
        {
            _verticalRotation = rotation.x;
            _horizontalRotation = rotation.y;

            _verticalRotation -= _rotateVerticalSpeed * _parameters.CameraSensitivity * _deltaMousePositionY;
            _horizontalRotation += _rotateHorizontalSpeed * _parameters.CameraSensitivity * _deltaMousePositionX;
            rotation = new Vector3(Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation),
                _horizontalRotation, 0.0f);

            return rotation;
        }

        private void ResetDeltas()
        {
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }
    }
}
