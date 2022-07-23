using Colonists;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls.CameraControls
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CameraRaising))]
    [RequireComponent(typeof(CameraThresholdMovement))]
    public class CameraFollowing : MonoBehaviour
    {
        private bool _following;

        private Transform _followTransform;
        private Vector3 _offset;

        private Colonist _colonist;

        private Camera _camera;

        private CameraRaising _cameraRaising;
        private CameraThresholdMovement _cameraThresholdMovement;

        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _setFollowAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _cameraRaising = GetComponent<CameraRaising>();
            _cameraThresholdMovement = GetComponent<CameraThresholdMovement>();

            _movementAction = _playerInput.actions.FindAction("Movement");
            _setFollowAction = _playerInput.actions.FindAction("Set Follow");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        public void Activate()
        {
            _setFollowAction.started += TryFollow;
        }

        public void Deactivate()
        {
            _setFollowAction.started -= TryFollow;
        }

        public void SetFollow(Colonist colonist)
        {
            UnsubscribeFromLastUnit();

            _colonist = colonist;
            _followTransform = colonist.transform;
            _offset = _newPosition - _followTransform.position;

            colonist.Dying += ResetFollow;

            _following = true;
        }

        public bool TryUpdateFollow()
        {
            var keyboardMoved = _movementAction.ReadValue<Vector2>() != Vector2.zero;

            var position = _mousePositionAction.ReadValue<Vector2>();

            var mouseMoved = _cameraThresholdMovement.IsMouseMoved(position);

            if (keyboardMoved || mouseMoved || _dragAction.IsPressed())
            {
                _following = false;
                return false;
            }
            else
            {
                UpdateFollow();
                return true;
            }
        }

        public void ResetFollow()
        {
            _followTransform = null;
            _following = false;
        }

        private void UpdateFollow()
        {
            transform.position = _cameraRaising.RaiseAboveTerrain(_followTransform.position + _offset);
            _newPosition = transform.position;
        }

        private void TryFollow(InputAction.CallbackContext context)
        {
            var screenPoint = _mousePositionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out var hit))
                if (hit.transform.gameObject.TryGetComponent<Colonist>(out var colonist))
                    SetFollow(colonist);
                else
                    ResetFollow();
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_colonist != null)
                _colonist.Dying -= ResetFollow;
        }
    }
}
