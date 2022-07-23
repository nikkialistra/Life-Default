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

        [SerializeField] private float _focusFov = 40f;
        [SerializeField] private float _focusDistance = 30f;
        [SerializeField] private float _focusRotation = 45f;
        [SerializeField] private float _focusDuration = .3f;
        [Space]
        [SerializeField] private float _minDistanceForTeleporation;

        private bool _focusing;

        private Coroutine _focusingCoroutine;

        private CameraFollowing _cameraFollowing;
        private CameraRaising _cameraRaising;

        private void Awake()
        {
            _cameraFollowing = GetComponent<CameraFollowing>();
            _cameraRaising = GetComponent<CameraRaising>();
        }

        public void FocusOn(Colonist colonist)
        {
            _cameraFollowing.ResetFollow();

            var yRotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            var forward = yRotation * Vector3.forward;

            var position = colonist.Center + (forward * -_focusDistance);
            position = _cameraRaising.RaiseAboveTerrain(position);

            var eulerAngles = new Vector3(_focusRotation, _newRotation.eulerAngles.y, _newRotation.eulerAngles.z);

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

        private void SetFocusRotation(Vector3 eulerAngles)
        {
            _newFieldOfView = _focusFov;
            _newRotation.eulerAngles = eulerAngles;
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
    }
}
