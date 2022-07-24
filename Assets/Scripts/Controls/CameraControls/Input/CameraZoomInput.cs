using UI.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Controls.CameraControls.Input
{
    public class CameraZoomInput : MonoBehaviour
    {
        [SerializeField] private float _zoomScrollSensitivity;

        [SerializeField] private float _minFov;
        [SerializeField] private float _maxFov;

        private GameViews _gameViews;

        private PlayerInput _playerInput;

        private InputAction _zoomScrollAction;

        [Inject]
        public void Construct(GameViews gameViews, PlayerInput playerInput)
        {
            _gameViews = gameViews;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _zoomScrollAction = _playerInput.actions.FindAction("Zoom Scroll");
        }

        public float UpdateZoom(float fieldOfView)
        {
            if (_gameViews.MouseOverUi)
                return fieldOfView;

            var zoomScroll = _zoomScrollAction.ReadValue<Vector2>().y;
            return Mathf.Clamp(fieldOfView - zoomScroll * _zoomScrollSensitivity,
                _minFov, _maxFov);
        }
    }
}
