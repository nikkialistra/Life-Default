using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Cameras
{
    public class CameraZooming : MonoBehaviour
    {
        [Title("Zoom")]
        [MinValue(0)]
        [SerializeField] private float _scrollZoomMultiplier;

        [MinValue(0)]
        [SerializeField] private float _buttonZoomMultiplier;

        [Title("Boundaries")]
        [ValidateInput("@_minimumPositionY < _maximumPositionY",
            "Minimum position should be less than maximum position")]
        [SerializeField] private float _minimumPositionY;
        [ValidateInput("@_minimumPositionY < _maximumPositionY",
            "Minimum position should be less than maximum position")]
        [SerializeField] private float _maximumPositionY;

        private Coroutine _zoomCoroutine;

        private PlayerInput _playerInput;

        private InputAction _zoomScrollAction;
        private InputAction _zoomAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _zoomScrollAction = _playerInput.actions.FindAction("Zoom Scroll");
            _zoomAction = _playerInput.actions.FindAction("Zoom");
        }

        public event Action<Vector3> PositionUpdate;

        public Vector3 Position { get; set; }

        private void OnDestroy()
        {
            Deactivate();
        }

        public void Activate()
        {
            _zoomScrollAction.started += ZoomScroll;
            _zoomAction.started += ZoomStart;
            _zoomAction.canceled += ZoomStop;
        }

        public void Deactivate()
        {
            _zoomScrollAction.started -= ZoomScroll;
            _zoomAction.started -= ZoomStart;
            _zoomAction.canceled -= ZoomStop;
        }

        private void ZoomScroll(InputAction.CallbackContext context)
        {
            var zooming = context.ReadValue<Vector2>().y;

            var localZoomAmount = transform.forward * zooming;
            var zoomPosition = Position + localZoomAmount * _scrollZoomMultiplier;

            ClampZoomByConstraints(zoomPosition);
        }

        private void ZoomStart(InputAction.CallbackContext context)
        {
            if (_zoomCoroutine != null)
            {
                StopCoroutine(_zoomCoroutine);
            }

            _zoomCoroutine = StartCoroutine(Zoom());
        }

        private IEnumerator Zoom()
        {
            while (true)
            {
                var zooming = _zoomAction.ReadValue<float>();
                if (zooming != 0)
                {
                    UpdateZoom(zooming);
                }

                yield return null;
            }
        }

        private void ZoomStop(InputAction.CallbackContext context)
        {
            if (_zoomCoroutine == null)
            {
                throw new InvalidOperationException();
            }

            StopCoroutine(_zoomCoroutine);
        }

        private void UpdateZoom(float zooming)
        {
            var localZoomAmount = transform.forward * zooming;
            var zoomPosition = Position + localZoomAmount * _buttonZoomMultiplier * Time.deltaTime;

            ClampZoomByConstraints(zoomPosition);
        }

        private void ClampZoomByConstraints(Vector3 zoomPosition)
        {
            if (zoomPosition.y < _minimumPositionY)
            {
                zoomPosition = CalculateAllowedPositionAtMinimum(zoomPosition);
            }

            if (zoomPosition.y > _maximumPositionY)
            {
                zoomPosition = CalculateAllowedPositionAtMaximum(zoomPosition);
            }

            Position = zoomPosition;
            PositionUpdate?.Invoke(Position);
        }

        private Vector3 CalculateAllowedPositionAtMinimum(Vector3 zoomPosition)
        {
            var allowedDistanceY = Position.y - _minimumPositionY;
            var actualDistanceY = Position.y - zoomPosition.y;

            var allowedDistancePercent = allowedDistanceY / actualDistanceY;

            var actualDistance = Position - zoomPosition;

            var zoomAllowedDelta = actualDistance * allowedDistancePercent;

            return Position - zoomAllowedDelta;
        }

        private Vector3 CalculateAllowedPositionAtMaximum(Vector3 zoomPosition)
        {
            var allowedDistanceY = _maximumPositionY - Position.y;
            var actualDistanceY = zoomPosition.y - Position.y;

            var allowedDistancePercent = allowedDistanceY / actualDistanceY;

            var actualDistance = zoomPosition - Position;

            var zoomAllowedDelta = actualDistance * allowedDistancePercent;

            return Position + zoomAllowedDelta;
        }
    }
}
