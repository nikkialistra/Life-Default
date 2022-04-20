using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Units.Ancillaries
{
    public class HealthBars : MonoBehaviour
    {
        [Required]
        [SerializeField] private Slider _healthSlider;
        [Required]
        [SerializeField] private Slider _recoverySpeedSlider;

        [Required]
        [SerializeField] private GameObject _sliders;

        private bool _shown;

        private GameObject _healthGameObject;
        private GameObject _recoverySpeedGameObject;
        
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

        public void SetHealth(float value)
        {
            _healthSlider.value = value;

            UpdateShowStatus();
        }
        
        public void SetRecoverySpeed(float value)
        {
            _recoverySpeedSlider.value = value;

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
            return _healthSlider.value == 0 || _recoverySpeedSlider.value == 0;
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
            return _healthSlider.value != 1f || _recoverySpeedSlider.value != 1f;
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
