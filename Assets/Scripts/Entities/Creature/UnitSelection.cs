using System.Collections;
using ColonistManagement.Selection;
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
        
        private Coroutine _hoveringCoroutine;
        
        private ColonistSelectionInput _colonistSelectionInput;

        public void Activate()
        {
            _activated = true;
        }

        public void Flash()
        {
            
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

        public void StopDisplay()
        {
            _activated = false;
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

                yield return new WaitForSecondsRealtime(_timeToHideHover);

                if (!_hovered)
                {
                    HideHoverIndicator();
                    break;
                }
            }

            _hoveringCoroutine = null;
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
