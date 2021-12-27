using System.Collections.Generic;
using System.Linq;
using Game.Units.Unit;
using Kernel.Selection;
using UnityEngine;
using Zenject;

namespace Game.Units.Selecting
{
    [RequireComponent(typeof(SelectionArea))]
    public class UnitsSelection : MonoBehaviour
    {
        [SerializeField] private float _doubleClickDeltaTime;

        private SelectionArea _selectionArea;
        
        private UnitsSelecting _selecting;
        private SelectionInput _selectionInput;
        private SelectedUnits _selectedUnits;

        private float _lastClickTime;

        [Inject]
        public void Construct(UnitsSelecting selecting, SelectionInput selectionInput, SelectedUnits selectedUnits)
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

        public void ClearSelection()
        {
            _selectedUnits.Clear();
        }

        private void Draw(Rect rect)
        {
            _selectionArea.Draw(rect);
        }

        private void Select(Rect rect)
        {
            var newSelected = GetSelected(rect);

            var newSelectedArray = newSelected as UnitFacade[] ?? newSelected.ToArray();
            foreach (var willDeselect in _selectedUnits.Units.Except(newSelectedArray))
            {
                willDeselect.Deselect();
            }

            foreach (var selected in newSelectedArray)
            {
                selected.Select();
            }

            _selectedUnits.Set(newSelectedArray.ToList());

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
            return rect.size == Vector2.zero;
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