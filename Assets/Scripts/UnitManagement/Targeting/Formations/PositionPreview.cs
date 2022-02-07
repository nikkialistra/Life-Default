using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnitManagement.Targeting.Formations
{
    public class PositionPreview : MonoBehaviour
    {
        [Required]
        [SerializeField] private DecalProjector _decalProjector;
        [Space]
        [MinValue(0)]
        [SerializeField] private float _fadeTime;

        private Coroutine _flashCoroutine;

        private void Start()
        {
            Activate();
        }

        public void StartFlash()
        {
            _decalProjector.fadeFactor = 1f;
            _flashCoroutine = StartCoroutine(Flash());
        }

        public bool Activated { get; private set; }
        public float FadeTime => _fadeTime;

        public void Activate()
        {
            Activated = true;
            _decalProjector.fadeFactor = 1f;
        }

        public void Deactivate()
        {
            Activated = false;
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }
        }

        private IEnumerator Flash()
        {
            var timeLeft = _fadeTime;

            while (timeLeft > 0)
            {
                var fraction = timeLeft / _fadeTime;
                _decalProjector.fadeFactor = fraction;

                timeLeft -= Time.deltaTime;

                yield return null;
            }

            _decalProjector.fadeFactor = 0f;
        }
    }
}
