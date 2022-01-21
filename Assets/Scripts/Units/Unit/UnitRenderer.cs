using System.Collections;
using Entities.Entity.Ancillaries;
using Entities.Entity.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Unit
{
    public class UnitRenderer : MonoBehaviour, IHoverable
    {
        [Required]
        [SerializeField] private SkinnedMeshRenderer _outlineRenderer;
        [Required]
        [SerializeField] private Material _outline;
        [MinValue(0)]
        [SerializeField] private float _waitingTimeValue = 0.05f;

        private WaitForSeconds _waitingTime;
        private bool _selected;

        private Coroutine _hideOutlineCoroutine;

        private void Awake()
        {
            _waitingTime = new WaitForSeconds(_waitingTimeValue);
        }

        public void Select()
        {
            _selected = true;

            ShowUnitOutline();
            ShowAccessoriesOutline();
        }

        public void Deselect()
        {
            _selected = false;

            HideUnitOutline();
            HideAccessoriesOutline();
        }

        public void OnHover()
        {
            if (_selected)
            {
                return;
            }

            if (_hideOutlineCoroutine != null)
            {
                StopCoroutine(_hideOutlineCoroutine);
            }

            ShowUnitOutline();
            ShowAccessoriesOutline();

            _hideOutlineCoroutine = StartCoroutine(HideOutline());
        }

        private IEnumerator HideOutline()
        {
            yield return _waitingTime;

            if (_selected)
            {
                yield break;
            }

            HideUnitOutline();
            HideAccessoriesOutline();
        }

        private void ShowUnitOutline()
        {
            _outlineRenderer.gameObject.SetActive(true);
        }

        private void HideUnitOutline()
        {
            _outlineRenderer.gameObject.SetActive(false);
        }

        private void ShowAccessoriesOutline()
        {
            var accessories = GetComponentsInChildren<Outlining>();
            foreach (var accessory in accessories)
            {
                accessory.Activate();
            }
        }

        private void HideAccessoriesOutline()
        {
            var accessories = GetComponentsInChildren<Outlining>();
            foreach (var accessory in accessories)
            {
                accessory.Deactivate();
            }
        }
    }
}
