using System.Collections.Generic;
using System.Linq;
using UnitManagement.Selection;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace Units.Services.Selecting
{
    [RequireComponent(typeof(SelectionArea))]
    public class UnitSelection : MonoBehaviour
    {
        [SerializeField] private float _doubleClickDeltaTime;

        private SelectionArea _selectionArea;

        private UnitSelecting _selecting;
        private SelectionInput _selectionInput;
        private SelectedUnits _selectedUnits;

        private float _lastClickTime;

        [Inject]
        public void Construct(UnitSelecting selecting, SelectionInput selectionInput, SelectedUnits selectedUnits)
        {
            _selectedUnits = selectedUnits;
            _selecting = selecting;
            _selectionInput = selectionInput;
        }

        private void Awake()
        {
            _selectionArea = GetComponent<SelectionArea>();
        }

        public void OnEnable()
        {
            _selectionInput.Selecting += Draw;
            _selectionInput.SelectingEnd += Select;
        }

        public void OnDisable()
        {
            _selectionInput.Selecting -= Draw;
            _selectionInput.SelectingEnd -= Select;
        }

        private void Draw(Rect rect)
        {
            if (!WasClick(rect))
            {
                _selectionArea.Draw(rect);
            }
        }

        private void Select(Rect rect)
        {
            var newSelected = GetSelected(rect).ToList();

            _selectedUnits.Set(newSelected);

            _selectionArea.StopDrawing();
        }

        private IEnumerable<UnitFacade> GetSelected(Rect rect)
        {
            if (WasClick(rect))
            {
                return GetSelectedFromClick(rect);
            }
            else
            {
                return _selecting.SelectFromRect(rect);
            }
        }

        private IEnumerable<UnitFacade> GetSelectedFromClick(Rect rect)
        {
            if (WasDoubleClick())
            {
                return SelectWithSameType();
            }
            else
            {
                _lastClickTime = Time.time;
                return _selecting.SelectFromPoint(rect.center);
            }
        }

        private static bool WasClick(Rect rect)
        {
            return (rect.width <= 8f && rect.height <= 8f);
        }

        private IEnumerable<UnitFacade> SelectWithSameType()
        {
            if (_selectedUnits.Units.Count() != 1)
            {
                return Enumerable.Empty<UnitFacade>();
            }

            return _selecting.SelectByType(_selectedUnits.Units.First().UnitType);
        }

        private bool WasDoubleClick()
        {
            return Time.time - _lastClickTime < _doubleClickDeltaTime;
        }
    }
}