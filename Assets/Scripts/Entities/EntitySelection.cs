using System.Collections;
using Entities.Interfaces;
using UnityEngine;

namespace Entities
{
    public class EntitySelection : MonoBehaviour, ISelectable
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private int _materialIndex;
        [SerializeField] private string _propertyName;
        [Space]
        [SerializeField] private float _timeToHideHover = 0.05f;
        [SerializeField] private float _timeToHideSelection = 0.12f;
        [Space]
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _selectionColor;

        private bool _hovered;
        private bool _selected;
        
        private int _emissiveColor;
        
        private Coroutine _hoveringCoroutine;
        private Coroutine _hideSelectionCoroutine;

        private void Start()
        {
            _emissiveColor = Shader.PropertyToID(_propertyName);
        }

        public void Hover()
        {
            if (_hovered || _selected)
            {
                return;
            }

            _hovered = true;
            
            _hoveringCoroutine ??= StartCoroutine(Hovering());
        }
        
        public void Flash()
        {
            StopDisplay();
            
            SetColor(_selectionColor);

            _hovered = false;
            _selected = true;

            _hideSelectionCoroutine = StartCoroutine(HideSelectionAfter());
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

        private IEnumerator Hovering()
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

        private IEnumerator HideSelectionAfter()
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
