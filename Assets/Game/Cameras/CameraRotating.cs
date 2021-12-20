using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Cameras
{
    public class CameraRotating : MonoBehaviour
    {
        [MinValue(0)] 
        [SerializeField] private float _rotationNormalSpeed;
        [SerializeField] private float _rotationFastSpeed;
        
        public event Action<Quaternion> RotationUpdate;
        
        public Quaternion Rotation { get; set; }

        private float _rotationSpeed;

        private Vector3? _rotateStartPosition;
        private Vector3 _rotateCurrentPosition;
        
        private Coroutine _rotateCoroutine;

        private PlayerInput _playerInput;
        
        private InputAction _rotationAction;
        private InputAction _rotateAction;
        private InputAction _fastCameraAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        
        private void Awake()
        {
            _rotateAction = _playerInput.actions.FindAction("Rotate");
            _fastCameraAction = _playerInput.actions.FindAction("FastCamera");
        }

        private void OnEnable()
        {
            _rotateAction.started += RotateStart;
            _rotateAction.canceled += RotateStop;
            
            _fastCameraAction.started += FastRotationOn;
            _fastCameraAction.canceled += FastRotationOff;
        }

        private void OnDisable()
        {
            _rotateAction.started -= RotateStart;
            _rotateAction.canceled -= RotateStop;
            
            _fastCameraAction.started -= FastRotationOn;
            _fastCameraAction.canceled -= FastRotationOff;
        }

        private void Start()
        {
            _rotationSpeed = _rotationNormalSpeed;
        }

        private void RotateStart(InputAction.CallbackContext context)
        {
            if (_rotateCoroutine != null)
            {
                StopCoroutine(_rotateCoroutine);
            }
            _rotateCoroutine = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            while (true)
            {
                var rotation = _rotateAction.ReadValue<float>();
                UpdateNewRotation(rotation * _rotationSpeed * Time.deltaTime);

                yield return null;
            }
        }

        private void UpdateNewRotation(float amount)
        {
            var newRotationEulerAngles = Rotation.eulerAngles;
            newRotationEulerAngles.y += amount;
            Rotation = Quaternion.Euler(newRotationEulerAngles);
            RotationUpdate?.Invoke(Rotation);
        }

        private void RotateStop(InputAction.CallbackContext context)
        {
            if (_rotateCoroutine == null)
            {
                throw new InvalidOperationException();
            }
            StopCoroutine(_rotateCoroutine);
        }

        private void FastRotationOn(InputAction.CallbackContext context)
        {
            _rotationSpeed = _rotationFastSpeed;
        }

        private void FastRotationOff(InputAction.CallbackContext context)
        {
            _rotationSpeed = _rotationNormalSpeed;
        }
    }
}