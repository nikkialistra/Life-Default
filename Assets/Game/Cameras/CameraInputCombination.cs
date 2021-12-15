using UnityEngine;

namespace Game.Cameras
{
    [RequireComponent(typeof(CameraFollowing))]
    [RequireComponent(typeof(CameraMoving))]
    [RequireComponent(typeof(CameraRotating))]
    [RequireComponent(typeof(CameraZooming))]
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

        private void Awake()
        {
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraMoving = GetComponent<CameraMoving>();
            _cameraRotating = GetComponent<CameraRotating>();
            _cameraZooming = GetComponent<CameraZooming>();
        }

        private void Start()
        {
            _newPosition = transform.position;
            _newRotation = transform.rotation;
            
            InitializeInputComponents(); 
        }

        private void OnEnable()
        {
            _cameraMoving.PositionUpdate += ApplyMoving;
            _cameraRotating.RotationUpdate += ApplyRotating;
            _cameraZooming.PositionUpdate += ApplyZooming;
        }

        private void OnDisable()
        {
            _cameraMoving.PositionUpdate -= ApplyMoving;
            _cameraRotating.RotationUpdate -= ApplyRotating;
            _cameraZooming.PositionUpdate -= ApplyZooming;
        }

        private void Update()
        {
            ComputeTransform();
            if (_cameraFollowing.Following)
            {
                _newPosition += _cameraFollowing.GetDeltaFollowPosition();
            }
        }

        private void ComputeTransform()
        {
            transform.position = Vector3.Lerp(transform.position, _newPosition, _positionSmoothing * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _rotationSmoothing * Time.deltaTime);
        }

        private void InitializeInputComponents()
        {
            _cameraMoving.Position = _newPosition;
            _cameraRotating.Rotation = _newRotation;
            _cameraZooming.Position = _newPosition;
        }

        private void ApplyMoving(Vector3 position)
        {
            _newPosition = position;
            _cameraZooming.Position = _newPosition;
        }

        private void ApplyRotating(Quaternion rotation)
        {
            _newRotation = rotation;
        }

        private void ApplyZooming(Vector3 position)
        {
            _newPosition = position;
            _cameraMoving.Position = _newPosition;
        }
    }
}