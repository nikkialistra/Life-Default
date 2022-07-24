using System.Collections;
using Colonists;
using DG.Tweening;
using UnityEngine;

namespace Controls.CameraControls
{
    [RequireComponent(typeof(CameraFollowing))]
    [RequireComponent(typeof(CameraRaising))]
    public class CameraFocusing : MonoBehaviour
    {
        public bool Focusing => _focusing;

        public Vector3 NewPosition => _newPosition;
        public Vector3 NewRotation => _newRotation;
        public float NewFieldOfView => _newFieldOfView;

        [SerializeField] private float _focusFov = 40f;
        [SerializeField] private float _focusDistance = 30f;
        [SerializeField] private float _focusRotation = 45f;
        [SerializeField] private float _focusDuration = .3f;
        [Space]
        [SerializeField] private float _minDistanceForTeleporation;

        private bool _focusing;

        private Vector3 _newPosition;
        private Vector3 _newRotation;
        private float _newFieldOfView;

        private Coroutine _focusingCoroutine;

        private CameraFollowing _cameraFollowing;
        private CameraRaising _cameraRaising;

        private void Awake()
        {
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraRaising = GetComponent<CameraRaising>();
        }

        public void FocusOn(Colonist colonist, Quaternion rotation)
        {
            _cameraFollowing.ResetFollow();

            var yRotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            var forward = yRotation * Vector3.forward;

            var position = colonist.Center + (forward * -_focusDistance);
            position = _cameraRaising.RaiseAboveTerrain(position);

            var eulerAngles = new Vector3(_focusRotation, rotation.eulerAngles.y, rotation.eulerAngles.z);

            StartFocusing(colonist, position, eulerAngles);
        }

        private void StartFocusing(Colonist colonist, Vector3 position, Vector3 eulerAngles)
        {
            if (_focusingCoroutine != null)
            {
                StopCoroutine(_focusingCoroutine);
                _focusingCoroutine = null;
            }

            if (Vector3.Distance(transform.position, colonist.Center) > _minDistanceForTeleporation)
            {
                transform.position = position;
                _newPosition = position;
                SetFocusRotation(eulerAngles);
            }
            else
            {
                _focusingCoroutine = StartCoroutine(CFocusing(position, eulerAngles, colonist));
            }
        }

        private IEnumerator CFocusing(Vector3 position, Vector3 eulerAngles, Colonist colonist)
        {
            _focusing = true;

            transform.DOMove(position, _focusDuration * Time.timeScale);

            yield return new WaitForSecondsRealtime(_focusDuration / 2f);

            // Start to change fov and rotation in the middle of movement
            SetFocusRotation(eulerAngles);

            yield return new WaitForSecondsRealtime(_focusDuration / 2f);

            _newPosition = transform.position;

            _cameraFollowing.SetFollow(colonist);

            _focusing = false;
        }

        private void SetFocusRotation(Vector3 eulerAngles)
        {
            _newFieldOfView = _focusFov;
            _newRotation = eulerAngles;
        }
    }
}
