using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.Ancillaries
{
    public class HealthBars : MonoBehaviour
    {
        [Required]
        [SerializeField] private Slider _vitalitySlider;
        [Required]
        [SerializeField] private Slider _bloodSlider;

        [Required]
        [SerializeField] private GameObject _sliders;

        private bool _shown;

        private GameObject _vitalityGameObject;
        private GameObject _bloodGameObject;
        
        private Transform _cameraTransform;
        private bool _selected;

        [Inject]
        public void Construct(Camera camera)
        {
            _cameraTransform = camera.transform;
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

        public void SetVitality(float value)
        {
            _vitalitySlider.value = value;

            UpdateShowStatus();
        }
        
        public void SetBlood(float value)
        {
            _bloodSlider.value = value;

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
            return _vitalitySlider.value == 0 || _bloodSlider.value == 0;
        }

        private void ShowIfSelectedOrHit()
        {
            if (Selected || NotFull())
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private bool NotFull()
        {
            return _vitalitySlider.value != 1f || _bloodSlider.value != 1f;
        }

        private void Show()
        {
            _sliders.SetActive(true);
        }

        private void Hide()
        {
            _sliders.SetActive(false);
        }
    }
}
