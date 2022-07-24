using System.Collections;
using Infrastructure.Settings;
using Selecting;
using UnityEngine;
using Zenject;

namespace Entities
{
    public class EntitySelection : MonoBehaviour, ISelectable
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private int _materialIndex;
        [SerializeField] private string _propertyName;
        [Space]
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _selectionColor;

        private float _timeToHideHover;
        private float _timeToHideSelection;

        private bool _hovered;
        private bool _selected;

        private int _emissiveColor;

        private Coroutine _hoveringCoroutine;
        private Coroutine _hideSelectionCoroutine;

        [Inject]
        public void Construct(SelectionSettings selectionSettings)
        {
            _timeToHideHover = selectionSettings.TimeToHideHover;
            _timeToHideSelection = selectionSettings.TimeToHideSelection;
        }

        private void Start()
        {
            _emissiveColor = Shader.PropertyToID(_propertyName);
        }

        public void Hover()
        {
            if (_hovered || _selected) return;

            _hovered = true;

            _hoveringCoroutine ??= StartCoroutine(CHovering());
        }

        public void Flash()
        {
            StopDisplay();

            SetColor(_selectionColor);

            _hovered = false;
            _selected = true;

            _hideSelectionCoroutine = StartCoroutine(CHideSelectionAfter());
        }

        public void Select()
        {
            StopDisplay();

            _hovered = false;
            _selected = true;

            SetColor(_selectionColor);
        }

        public void Deselect()
        {
            _selected = false;
            SetColor(Color.black);
        }

        public void StopDisplay()
        {
            if (_hoveringCoroutine != null)
            {
                StopCoroutine(_hoveringCoroutine);
                _hoveringCoroutine = null;
            }

            if (_hideSelectionCoroutine != null)
            {
                StopCoroutine(_hideSelectionCoroutine);
                _hideSelectionCoroutine = null;
            }
        }

        private IEnumerator CHovering()
        {
            SetColor(_hoverColor);

            while (true)
            {
                _hovered = false;

                yield return new WaitForSecondsRealtime(_timeToHideHover);

                if (!_hovered)
                {
                    SetColor(Color.black);
                    break;
                }
            }

            _hoveringCoroutine = null;
        }

        private IEnumerator CHideSelectionAfter()
        {
            yield return new WaitForSecondsRealtime(_timeToHideSelection);

            SetColor(Color.black);
            _selected = false;
        }

        private void SetColor(Color color)
        {
            _renderer.materials[_materialIndex].SetColor(_emissiveColor, color);
        }
    }
}
