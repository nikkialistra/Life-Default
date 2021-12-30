using System;
using Units.Services.Selecting;
using Units.Unit;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollowing : MonoBehaviour
    {
        public event Action<Vector3> PositionUpdate;
        
        public bool Following { get; private set; }

        private UnitFacade _unit;
        private Transform _followTransform;
        private Vector3 _followOffset;

        private Camera _camera;
        private Vector3 _followLastPosition;

        private UnitsChoosing _unitsChoosing;

        private PlayerInput _playerInput;

        private InputAction _setFollowAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(UnitsChoosing unitsChoosing, PlayerInput playerInput)
        {
            _unitsChoosing = unitsChoosing;
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
            _unitsChoosing.UnitChosen += SetFollow;
        }

        private void OnDisable()
        {
            _setFollowAction.started -= TryFollow;
            _unitsChoosing.UnitChosen -= SetFollow;
        }

        private void OnDestroy()
        {
            UnsubscribeFromLastUnit();
        }

        private void TryFollow(InputAction.CallbackContext context)
        {
            var screenPoint = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<UnitFacade>(out var unit))
                {
                    SetFollow(unit);
                }
                else
                {
                    ResetFollow();
                }
            }
        }

        private void SetFollow(UnitFacade unit)
        {
            UnsubscribeFromLastUnit();
            
            _unit = unit;
            _followTransform = unit.transform;
            
            unit.Die += ResetFollow;
            
            UpdateCameraPosition();
            _followLastPosition = _followTransform.position;
            Following = true;
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

        public void ResetFollow()
        {
            _followTransform = null;
            Following = false;
        }
        
        private void UnsubscribeFromLastUnit()
        {
            if (_unit != null)
            {
                _unit.Die -= ResetFollow;
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
    }
}