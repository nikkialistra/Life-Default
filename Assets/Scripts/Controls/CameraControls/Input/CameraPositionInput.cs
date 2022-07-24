using System.Collections;
using General.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls.CameraControls.Input
{
    [RequireComponent(typeof(CameraParameters))]
    public class CameraPositionInput : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 40f;

        private bool _canMouseScroll;

        private bool _isSelectingInput;

        private CameraThresholdMovement _cameraThresholdMovement;

        private CameraParameters _parameters;

        private SelectingInput _selectingInput;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(SelectingInput selectingInput, PlayerInput playerInput)
        {
            _selectingInput = selectingInput;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _cameraThresholdMovement = GetComponent<CameraThresholdMovement>();

            _parameters = GetComponent<CameraParameters>();

            _movementAction = _playerInput.actions.FindAction("Movement");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        private void OnEnable()
        {
            _selectingInput.Selecting += OnSelecting;
            _selectingInput.SelectingEnd += OnSelectingEnd;
        }

        private void OnDisable()
        {
            _selectingInput.Selecting -= OnSelecting;
            _selectingInput.SelectingEnd -= OnSelectingEnd;
        }

        public void Activate()
        {
            StartCoroutine(CAllowMouseScrollALittleLater());
        }

        public void Deactivate()
        {
            _canMouseScroll = false;
        }

        public Vector3 UpdatePosition(Vector3 position)
        {
            position += DeltaPositionFromKeyboard();
            position += DeltaPositionFromMouseThresholdMovement();

            return position;
        }

        private IEnumerator CAllowMouseScrollALittleLater()
        {
            yield return new WaitForSeconds(0.1f);
            _canMouseScroll = true;
        }

        private Vector3 DeltaPositionFromKeyboard()
        {
            var movement = _movementAction.ReadValue<Vector2>();

            var delta = new Vector3();

            delta += transform.right * (_moveSpeed * Time.unscaledDeltaTime * movement.x);
            delta += transform.forward * (_moveSpeed * Time.unscaledDeltaTime * movement.y);

            return delta;
        }

        private Vector3 DeltaPositionFromMouseThresholdMovement()
        {
            if (!_canMouseScroll || !_parameters.ScreenEdgeMouseScroll || _isSelectingInput)
                return Vector3.zero;

            var position = _mousePositionAction.ReadValue<Vector2>();

            var movement = _cameraThresholdMovement.GetMovementFromPosition(position, _moveSpeed);

            return movement != Vector2.zero ? UpdatePositionFromMouseMovement(movement) : Vector3.zero;
        }

        private Vector3 UpdatePositionFromMouseMovement(Vector2 movement)
        {
            var delta = new Vector3();

            delta += transform.right * movement.x;
            delta += transform.up * movement.y + transform.forward * movement.y;

            return delta;
        }

        private void OnSelecting(Rect _)
        {
            _isSelectingInput = true;
        }

        private void OnSelectingEnd(Rect _)
        {
            _isSelectingInput = false;
        }
    }
}
