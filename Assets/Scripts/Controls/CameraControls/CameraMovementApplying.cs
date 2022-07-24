using Sirenix.OdinInspector;
using UnityEngine;

namespace Controls.CameraControls
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CameraMovement))]
    [RequireComponent(typeof(CameraRaising))]
    public class CameraMovementApplying : MonoBehaviour
    {
        [Title("Boundaries")]
        [SerializeField] private float _minimumPositionX;
        [SerializeField] private float _maximumPositionX;
        [Space]
        [SerializeField] private float _minimumPositionZ;
        [SerializeField] private float _maximumPositionZ;

        [Title("Smoothing")]
        [SerializeField] private float _positionSmoothing;
        [SerializeField] private float _rotationSmoothing;
        [SerializeField] private float _zoomSmoothing;

        private Camera _camera;

        private CameraMovement _cameraMovement;
        private CameraRaising _cameraRaising;

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _cameraMovement = GetComponent<CameraMovement>();
            _cameraRaising = GetComponent<CameraRaising>();
        }

        public Vector3 ClampPositionByConstraints()
        {
           var position = _cameraRaising.RaiseAboveTerrain(_cameraMovement.NewPosition);

            if (position.x < _minimumPositionX)
                position.x = _minimumPositionX;

            if (position.x > _maximumPositionX)
                position.x = _maximumPositionX;

            if (position.z < _minimumPositionZ)
                position.z = _minimumPositionZ;

            if (position.z > _maximumPositionZ)
                position.z = _maximumPositionZ;

            return position;
        }

        public void SmoothUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _cameraMovement.NewPosition,
                _positionSmoothing * Time.unscaledDeltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_cameraMovement.NewRotation),
                _rotationSmoothing * Time.unscaledDeltaTime);

            _camera.fieldOfView =
                Mathf.Lerp(_camera.fieldOfView, _cameraMovement.NewFieldOfView, _zoomSmoothing * Time.unscaledDeltaTime);
        }
    }
}
