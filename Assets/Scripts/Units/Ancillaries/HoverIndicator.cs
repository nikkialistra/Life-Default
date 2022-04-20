using UnityEngine;

namespace Units.Ancillaries
{
    public class HoverIndicator : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;

        private bool _activated;

        public void Activate()
        {
            if (_activated)
            {
                return;
            }
            
            _activated = true;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            if (!_activated)
            {
                return;
            }
            
            _activated = false;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_activated)
            {
                transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
            }
        }
    }
}
