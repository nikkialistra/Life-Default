using System;
using System.Collections;
using DG.Tweening;
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
        [SerializeField] private float _animationTime;
        [SerializeField] private AnimationKind _animationKind = AnimationKind.Collapse;

        private Coroutine _animateCoroutine;

        private void Start()
        {
            Activate();
        }

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

        public void Activate()
        {
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
            yield return new WaitForSeconds(_animationTime);

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
