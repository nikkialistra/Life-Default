using System;
using System.Collections;
using General;
using General.Interfaces;
using General.Selecting;
using Sirenix.OdinInspector;
using Units.Ancillaries;
using UnityEngine;

namespace Units
{
    public class UnitSelection : MonoBehaviour, ISelectable
    {
        [Required]
        [SerializeField] private LandIndicator _hoverIndicator;
        [Required]
        [SerializeField] private HealthBars _healthBars;

        private float _timeToHideHover;

        private bool _hovered;
        private bool _selected;
        private bool _activated;
        
        private Coroutine _hoveringCoroutine;
        
        private SelectingInput _selectingInput;

        public event Action Hovered;
        public event Action Unhovered;
        public event Action Selected;
        public event Action Deselected;

        private void Start()
        {
            _timeToHideHover = GlobalParameters.Instance.TimeToHideHover;
        }

        public void Flash()
        {
            
        }

        public void Select()
        {
            _selected = true;
            HideHovering();
            
            Selected?.Invoke();
        }

        public void Deselect()
        {
            _selected = false;

            Deselected?.Invoke();
        }

        public void StopDisplay()
        {
            _activated = false;
            HideHovering();
        }

        public void Hover()
        {
            if (_hovered || _selected || !_activated)
            {
                return;
            }

            _hovered = true;

            if (_hoveringCoroutine == null)
            {
                _hoveringCoroutine = StartCoroutine(Hovering());
            }

            Hovered?.Invoke();
        }
        
        public void Activate()
        {
            _activated = true;
        }

        public void Deactivate()
        {
            _activated = false;
            Unhovered?.Invoke();
        }

        private IEnumerator Hovering()
        {
            ShowHovering();
            
            while (true)
            {
                _hovered = false;

                yield return new WaitForSecondsRealtime(_timeToHideHover);

                if (!_hovered)
                {
                    HideHovering();
                    break;
                }
            }

            _hoveringCoroutine = null;
        }

        private void ShowHovering()
        {
            _hoverIndicator.Activate();
            _healthBars.Hovered = true;
        }

        private void HideHovering()
        {
            _hoverIndicator.Deactivate();
            _healthBars.Hovered = false;
            
            Unhovered?.Invoke();
        }
    }
}
