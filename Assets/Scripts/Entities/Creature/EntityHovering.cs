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
        [MinValue(0)]
        [SerializeField] private float _waitingTimeValue = 0.05f;

        private WaitForSeconds _waitingTime;
        private bool _selected;

        private Coroutine _hideHoverIndicatorCoroutine;

        private void Awake()
        {
            _waitingTime = new WaitForSeconds(_waitingTimeValue);
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
            if (_selected)
            {
                return;
            }

            if (_hideHoverIndicatorCoroutine != null)
            {
                StopCoroutine(_hideHoverIndicatorCoroutine);
            }

            ShowHoverIndicator();

            _hideHoverIndicatorCoroutine = StartCoroutine(HideHoverIndicatorAfter());
        }

        private IEnumerator HideHoverIndicatorAfter()
        {
            yield return _waitingTime;

            HideHoverIndicator();
        }

        private void ShowHoverIndicator()
        {
            _hoverIndicator.Activate();
        }

        private void HideHoverIndicator()
        {
            _hoverIndicator.Deactivate();
        }
    }
}
