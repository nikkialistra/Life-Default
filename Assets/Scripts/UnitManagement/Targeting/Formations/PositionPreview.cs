using System;
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

        public void StartFlash(Action<PositionPreview> onFinish)
        {
            _decalProjector.fadeFactor = 1f;
            StartCoroutine(Flash(onFinish));
        }

        private IEnumerator Flash(Action<PositionPreview> onFinish)
        {
            _decalProjector.fadeFactor = 1f;

            var timeLeft = _fadeTime;

            while (timeLeft > 0)
            {
                var fraction = timeLeft / _fadeTime;
                _decalProjector.fadeFactor = fraction;

                timeLeft -= Time.deltaTime;

                yield return null;
            }

            _decalProjector.fadeFactor = 0f;
            onFinish(this);
        }
    }
}
