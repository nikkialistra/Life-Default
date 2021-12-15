using UnityEngine;

namespace Game.Cameras
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CameraFollowing))]
    [RequireComponent(typeof(CameraZooming))]
    public class CameraInputCombination : MonoBehaviour
    {
        [SerializeField] private float _positionSmoothing;
        [SerializeField] private float _rotationSmoothing;

        private Camera _camera;

        private CameraFollowing _cameraFollowing;
        private CameraZooming _cameraZooming;

        private Vector3 _newPosition;
        private Quaternion _newRotation;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraZooming = GetComponent<CameraZooming>();
        }

        private void Start()
        {
            _newPosition = transform.position;
            _newRotation = transform.rotation;
            
            UpdateInputValues(); 
        }

        private void OnEnable()
        {
            _cameraZooming.PositionUpdate += ApplyZoom;
        }

        private void OnDisable()
        {
            _cameraZooming.PositionUpdate -= ApplyZoom;
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

        private void UpdateInputValues()
        {
            _cameraZooming.Position = _newPosition;
        }

        private void ApplyZoom(Vector3 position)
        {
            _newPosition = position;
            UpdateInputValues();
        }
    }
}