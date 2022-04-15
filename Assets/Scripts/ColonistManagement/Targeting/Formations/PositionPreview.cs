using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ColonistManagement.Targeting.Formations
{
    public class PositionPreview : MonoBehaviour
    {
        [Required]
        [SerializeField] private DecalProjector _decalProjector;
        [Space]
        [MinValue(0)]
        [SerializeField] private float _animationTime;
        [EnumToggleButtons]
        [SerializeField] private AnimationKind _animationKind = AnimationKind.Collapse;

        [Title("Materials")]
        [SerializeField] private Material _white;
        [SerializeField] private Material _red;

        private Coroutine _animateCoroutine;

        public void StartAnimation()
        {
            _animateCoroutine = _animationKind switch
            {
                AnimationKind.Collapse => StartCoroutine(Collapse()),
                AnimationKind.Fade => StartCoroutine(Fade()),
                AnimationKind.Hide => StartCoroutine(Hide()),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool Activated { get; private set; }
        public float AnimationTime => _animationTime;

        public void Activate(FormationColor formationColor)
        {
            _decalProjector.transform.DOKill();

            _decalProjector.material = formationColor switch
            {
                FormationColor.White => _white,
                FormationColor.Red => _red,
                _ => throw new ArgumentOutOfRangeException(nameof(formationColor), formationColor, null)
            };

            Activated = true;
            _decalProjector.transform.localScale = Vector3.one;
            _decalProjector.fadeFactor = 1f;
        }

        public void Deactivate()
        {
            Activated = false;
            if (_animateCoroutine != null)
            {
                StopCoroutine(_animateCoroutine);
                _animateCoroutine = null;
            }
        }

        private IEnumerator Collapse()
        {
            _decalProjector.transform.localScale = Vector3.one;

            _decalProjector.transform.DOKill();

            yield return _decalProjector.transform.DOScale(new Vector3(0f, 0f, 1f), _animationTime)
                .WaitForCompletion();
        }

        private IEnumerator Fade()
        {
            var timeLeft = _animationTime;

            while (timeLeft > 0)
            {
                var fraction = timeLeft / _animationTime;
                _decalProjector.fadeFactor = fraction;

                timeLeft -= Time.deltaTime;

                yield return null;
            }

            _decalProjector.fadeFactor = 0f;
        }

        private IEnumerator Hide()
        {
            yield return new WaitForSecondsRealtime(_animationTime);

            _decalProjector.fadeFactor = 0f;
        }

        private enum AnimationKind
        {
            Collapse,
            Fade,
            Hide
        }
    }
}
