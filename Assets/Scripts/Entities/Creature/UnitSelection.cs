using System.Collections;
using Entities.Ancillaries;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Creature
{
    public class UnitSelection : MonoBehaviour, ISelectable
    {
        [Required]
        [SerializeField] private HoverIndicator _hoverIndicator;
        [Required]
        [SerializeField] private HealthBars _healthBars;
        [MinValue(0)]
        [SerializeField] private float _timeToHideHover = 0.05f;

        private bool _hovered;
        private bool _selected;
        private bool _activated;

        private WaitForSeconds _hoveringHideTime;
        private Coroutine _hoveringCoroutine;

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

        public void Hover()
        {
            if (_hovered || _selected || !_activated)
            {
                return;
            }

            _hovered = true;

            _hoveringCoroutine ??= StartCoroutine(Hovering());
        }

        private IEnumerator Hovering()
        {
            ShowHoverIndicator();
            
            while (true)
            {
                _hovered = false;
                
                yield return _hoveringHideTime;

                if (!_hovered)
                {
                    HideHoverIndicator();
                    _hovered = false;
                    break;
                }
            }
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
