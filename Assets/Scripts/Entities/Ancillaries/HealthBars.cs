using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.Ancillaries
{
    public class HealthBars : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

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

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                UpdateShowStatus();
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraTransform.forward);
        }

        public void SetHealth(float value)
        {
            _slider.value = value;

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
