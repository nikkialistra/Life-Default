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
        [SerializeField] private float _steppedRotationAmount;
        
        public event Action<Quaternion> RotationUpdate;
        
        public Quaternion Rotation { get; set; }

        private Vector3? _rotateStartPosition;
        private Vector3 _rotateCurrentPosition;
        
        private Coroutine _rotateCoroutine;

        private PlayerInput _playerInput;
        
        private InputAction _rotationAction;
        private InputAction _rotateAction;
        
        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        
        private void Awake()
        {
            _rotateAction = _playerInput.actions.FindAction("Rotate");
        }

        private void OnEnable()
        {
            _rotateAction.started += RotateStart;
            _rotateAction.canceled += RotateStop;
        }

        private void OnDisable()
        {
            _rotateAction.started -= RotateStart;
            _rotateAction.canceled -= RotateStop;
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
                UpdateNewRotation(rotation * _steppedRotationAmount * Time.deltaTime);

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
    }
}