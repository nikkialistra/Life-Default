using MapGeneration.Map;
using UnityEngine;
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

        [Inject]
        public void Construct(bool isSetUpSession, Map map)
        {
            _isSetUpSession = isSetUpSession;
            _map = map;
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
                return;
            }

            _map.Load += OnMapLoad;

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

        private void OnMapLoad()
        {
            _map.Load -= OnMapLoad;

            ActivateComponents();
        }

        public void ActivateComponents()
        {
            _cameraFollowing.Activate();
            _cameraMoving.Activate();
            _cameraRotating.Activate();
            _cameraZooming.Activate();
        }

        public void DeactivateComponents()
        {
            _cameraFollowing.Deactivate();
            _cameraMoving.Deactivate();
            _cameraRotating.Deactivate();
            _cameraZooming.Deactivate();
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
