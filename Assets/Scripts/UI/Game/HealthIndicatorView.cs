using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Game
{
    public class HealthIndicatorView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fill;
        [SerializeField] private Gradient _fillGradient;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                UpdateShowStatus();
            }
        }

        private bool _shown;

        private GameObject _sliderGameObject;
        private Transform _cameraTransform;
        private bool _selected;

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

            UpdateShowStatus();
        }

        private void UpdateShowStatus()
        {
            if (Dead())
            {
                Hide();
            }
            else
            {
                ShowIfSelectedOrHit();
            }
        }

        private bool Dead()
        {
            return _slider.normalizedValue == 0;
        }

        private void ShowIfSelectedOrHit()
        {
            if (Selected || _slider.normalizedValue != 1)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void SetMaxHealth(int health)
        {
            _slider.maxValue = health;
            _slider.value = health;

            _fill.color = _fillGradient.Evaluate(1f);
        }

        private void Show()
        {
            _sliderGameObject.SetActive(true);
        }

        private void Hide()
        {
            _sliderGameObject.SetActive(false);
        }
    }
}