using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Zenject;

namespace Testing
{
    [RequireComponent(typeof(Camera))]
    public class FlyCamera : MonoBehaviour
    {
        [SerializeField] private float _acceleration = 50;
        [SerializeField] private float _accSprintMultiplier = 4;
        [SerializeField] private float _lookSensitivity = 1;
        [SerializeField] private float _dampingCoefficient = 5;
        [SerializeField] private bool _focusOnEnable = true;

        [SerializeField] private Vector3 _velocity;

        private Camera _camera;

        private bool _isSetUpSession;

        [Inject]
        public void Construct(bool isSetUpSession)
        {
            _isSetUpSession = isSetUpSession;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private static bool Focused
        {
            get => Cursor.lockState == CursorLockMode.Locked;
            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = value == false;
            }
        }

        private void Start()
        {
            if (!_isSetUpSession)
            {
                _camera.enabled = false;
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_focusOnEnable)
            {
                Focused = true;
            }
        }

        private void OnDisable()
        {
            Focused = false;
        }

        private void Update()
        {
            if (Focused)
            {
                UpdateInput();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                Focused = true;
            }

            _velocity = Vector3.Lerp(_velocity, Vector3.zero, _dampingCoefficient * Time.deltaTime);
            transform.position += _velocity * Time.deltaTime;
        }

        private void UpdateInput()
        {
            _velocity += GetAccelerationVector() * Time.deltaTime;

            var mouseDeltaRaw = Mouse.current.delta.ReadValue();
            var mouseDelta = _lookSensitivity * new Vector2(mouseDeltaRaw.x, -mouseDeltaRaw.y);

            var rotation = transform.rotation;
            var horizontal = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
            var vertical = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);

            transform.rotation = horizontal * rotation * vertical;

            if (Keyboard.current.altKey.isPressed && Keyboard.current.qKey.wasPressedThisFrame)
            {
                Focused = false;
            }
        }

        private Vector3 GetAccelerationVector()
        {
            Vector3 moveInput = default;

            void AddMovementIfPressed(KeyControl key, Vector3 dir)
            {
                if (key.isPressed)
                {
                    moveInput += dir;
                }
            }

            AddMovementIfPressed(Keyboard.current.wKey, Vector3.forward);
            AddMovementIfPressed(Keyboard.current.sKey, Vector3.back);
            AddMovementIfPressed(Keyboard.current.dKey, Vector3.right);
            AddMovementIfPressed(Keyboard.current.aKey, Vector3.left);
            AddMovementIfPressed(Keyboard.current.spaceKey, Vector3.up);
            AddMovementIfPressed(Keyboard.current.leftCtrlKey, Vector3.down);

            var direction = transform.TransformVector(moveInput.normalized);

            if (Keyboard.current.leftShiftKey.isPressed)
            {
                return direction * (_acceleration * _accSprintMultiplier);
            }

            return direction * _acceleration;
        }
    }
}
