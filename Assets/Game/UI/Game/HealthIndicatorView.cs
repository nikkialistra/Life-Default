using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.Game
{
    public class HealthIndicatorView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fill;
        [SerializeField] private Gradient _fillGradient;

        private bool _shown;

        private GameObject _sliderGameObject;
        private Transform _cameraTransform;

        [Inject]
        public void Construct(Camera camera)
        {
            _cameraTransform = camera.transform;
        }

        private void Awake()
        {
            _sliderGameObject = _slider.gameObject;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraTransform.forward);
        }

        public void SetHealth(int value)
        {
            _slider.value = value;
            _fill.color = _fillGradient.Evaluate(_slider.normalizedValue);
        }

        public void SetMaxHealth(int health)
        {
            _slider.maxValue = health;
            _slider.value = health;

            _fill.color = _fillGradient.Evaluate(1f);
        }

        public void Show()
        {
            _sliderGameObject.SetActive(true);
        }

        public void Hide()
        {
            _sliderGameObject.SetActive(false);
        }
    }
}