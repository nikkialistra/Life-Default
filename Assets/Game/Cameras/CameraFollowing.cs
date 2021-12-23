using System;
using Game.Units;
using Kernel.Types;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollowing : MonoBehaviour
    {
        public event Action<Vector3> PositionUpdate;
        
        public bool Following { get; private set; }
        
        private Transform _followTransform;
        private Vector3 _followOffset;
        
        private Camera _camera;
        private Vector3 _followLastPosition;
        
        private PlayerInput _playerInput;
        
        private InputAction _setFollowAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            
            _setFollowAction = _playerInput.actions.FindAction("SetFollow");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        private void OnEnable()
        {
            _setFollowAction.started += TryFollow;
        }

        private void OnDisable()
        {
            _setFollowAction.started -= TryFollow;
        }

        public Vector3 GetDeltaFollowPosition()
        {
            if (!Following)
            {
                throw new InvalidOperationException("Trying to get follow position while not following.");
            }

            var delta = _followTransform.position - _followLastPosition;
            _followLastPosition = _followTransform.position;
            return delta;
        }

        private void TryFollow(InputAction.CallbackContext context)
        {
            var screenPoint = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.GetComponent<UnitFacade>() != null)
                {
                    _followTransform = hit.transform;
                    UpdateCameraPosition();
                    _followLastPosition = _followTransform.position;
                    Following = true;
                }
                else
                {
                    ResetFollow();
                }
            }
        }

        private void UpdateCameraPosition()
        {
            var yDifference = transform.position.y - _followTransform.position.y;
            var yChangeWhenLookingOnUnit = (transform.rotation * Vector3.back).y;
            var distanceMultiplier = yDifference /  yChangeWhenLookingOnUnit;
            
            var cameraPosition = _followTransform.position - (transform.rotation * Vector3.forward * distanceMultiplier);
            PositionUpdate?.Invoke(cameraPosition);
        }

        public void ResetFollow()
        {
            _followTransform = null;
            Following = false;
        }
    }
}