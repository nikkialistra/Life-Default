using System.Collections;
using Entities.Ancillaries;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Creature
{
    public class EntityHovering : MonoBehaviour, IHoverable
    {
        [Required]
        [SerializeField] private HoverIndicator _hoverIndicator;
        [Required]
        [SerializeField] private HealthBars _healthBars;
        [MinValue(0)]
        [SerializeField] private float _timeToHideHover = 0.05f;

        private bool _selected;
        
        private WaitForSeconds _hoveringHideTime;
        private Coroutine _hideHoveringCoroutine;

        private bool _activated;

        private void Start()
        {
            _hoveringHideTime = new WaitForSeconds(_timeToHideHover);
        }

        public void Activate()
        {
            _activated = true;
        }

        public void Deactivate()
        {
            _activated = false;
            HideHoverIndicator();
        }

        public void Select()
        {
            _selected = true;
            HideHoverIndicator();
        }

        public void Deselect()
        {
            _selected = false;
            HideHoverIndicator();
        }

        public void OnHover()
        {
            if (!_activated || _selected)
            {
                return;
            }

            if (_hideHoveringCoroutine != null)
            {
                StopCoroutine(_hideHoveringCoroutine);
            }

            ShowHoverIndicator();

            _hideHoveringCoroutine = StartCoroutine(HideHoveringAfter());
        }

        private IEnumerator HideHoveringAfter()
        {
            yield return _hoveringHideTime;

            HideHoverIndicator();
        }

        private void ShowHoverIndicator()
        {
            _hoverIndicator.Activate();
            _healthBars.Selected = true;
        }

        private void HideHoverIndicator()
        {
            _hoverIndicator.Deactivate();
            _healthBars.Selected = false;
        }
    }
}
