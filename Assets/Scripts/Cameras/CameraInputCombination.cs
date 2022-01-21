using MapGeneration.Map;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Cameras
{
    [RequireComponent(typeof(CameraFollowing))]
    [RequireComponent(typeof(CameraMoving))]
    [RequireComponent(typeof(CameraRotating))]
    [RequireComponent(typeof(CameraZooming))]
    [RequireComponent(typeof(Camera))]
    public class CameraInputCombination : MonoBehaviour
    {
        [SerializeField] private float _positionSmoothing;
        [SerializeField] private float _rotationSmoothing;

        private CameraFollowing _cameraFollowing;
        private CameraMoving _cameraMoving;
        private CameraRotating _cameraRotating;
        private CameraZooming _cameraZooming;

        private Vector3 _newPosition;
        private Quaternion _newRotation;

        private Camera _camera;

        private bool _isSetUpSession;
        private Map _map;

        private PlayerInput _playerInput;

        [Inject]
        public void Construct(bool isSetUpSession, Map map, PlayerInput playerInput)
        {
            _isSetUpSession = isSetUpSession;
            _map = map;

            _playerInput = playerInput;
        }

        private void Awake()
        {
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraMoving = GetComponent<CameraMoving>();
            _cameraRotating = GetComponent<CameraRotating>();
            _cameraZooming = GetComponent<CameraZooming>();
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            if (_isSetUpSession)
            {
                _camera.enabled = false;
            }

            _playerInput.enabled = false;
            _map.Load += Activate;

            _newPosition = transform.position;
            _newRotation = transform.rotation;

            _cameraRotating.Rotation = _newRotation;

            SetInputPositionComponents();
        }

        private void OnEnable()
        {
            _cameraFollowing.PositionUpdate += ApplyFollowing;
            _cameraMoving.PositionUpdate += ApplyMoving;
            _cameraRotating.RotationUpdate += ApplyRotating;
            _cameraZooming.PositionUpdate += ApplyZooming;
        }

        private void OnDisable()
        {
            _cameraFollowing.PositionUpdate += ApplyFollowing;
            _cameraMoving.PositionUpdate -= ApplyMoving;
            _cameraRotating.RotationUpdate -= ApplyRotating;
            _cameraZooming.PositionUpdate -= ApplyZooming;
        }

        private void Activate()
        {
            _map.Load -= Activate;

            if (_isSetUpSession)
            {
                return;
            }

            _playerInput.enabled = true;
        }

        private void LateUpdate()
        {
            ComputeTransform();
            if (_cameraFollowing.Following)
            {
                _newPosition += _cameraFollowing.GetDeltaFollowPosition();
                SetInputPositionComponents();
            }
        }

        private void ComputeTransform()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition, _positionSmoothing * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationSmoothing * Time.deltaTime);
        }

        private void SetInputPositionComponents()
        {
            _cameraMoving.Position = _newPosition;
            _cameraZooming.Position = _newPosition;
        }

        private void ApplyFollowing(Vector3 position)
        {
            _newPosition = position;
            SetInputPositionComponents();
        }

        private void ApplyMoving(Vector3 position)
        {
            _newPosition = position;
            _cameraFollowing.ResetFollow();
            SetInputPositionComponents();
        }

        private void ApplyRotating(Quaternion rotation)
        {
            _newRotation = rotation;
            _cameraFollowing.ResetFollow();
        }

        private void ApplyZooming(Vector3 position)
        {
            _newPosition = position;
            SetInputPositionComponents();
        }
    }
}
