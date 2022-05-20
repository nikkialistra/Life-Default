using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace Units.Ancillaries
{
    public class MessageShowing : MonoBehaviour
    {
        [Required]
        [SerializeField] private TMP_Text _message;
        [Space]
        [SerializeField] private float _timeToFade = 0.8f;
        [SerializeField] private float _liftDistance = 0.5f;
        
        private Transform _cameraTransform;

        [Inject]
        public void Construct(Camera camera)
        {
            _cameraTransform = camera.transform;
        }

        private void Start()
        {
            _message.alpha = 0f;
        }
        
        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraTransform.forward);
        }

        public void Show(string message)
        {
            Show(message, Color.white);
        }

        public void Show(string message, Color color)
        {
            ResetAnimation();

            _message.text = message;
            _message.color = color;

            StartCoroutine(Fade());
        }

        private void ResetAnimation()
        {
            transform.position -= new Vector3(0, _liftDistance, 0);
            _message.alpha = 1f;
        }

        private IEnumerator Fade()
        {
            yield return null;
            
            transform.DOKill();
            _message.DOKill();
            
            StartFading();
        }

        private void StartFading()
        {
            transform.DOMoveY(transform.position.y + _liftDistance, _timeToFade);
            _message.DOFade(0.8f, _timeToFade / 2).OnComplete(FadeToZero);
        }

        private void FadeToZero()
        {
            _message.DOFade(0, _timeToFade / 2);
        }
    }
}
