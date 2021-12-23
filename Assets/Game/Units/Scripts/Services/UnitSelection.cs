using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Selection;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitSelection : MonoBehaviour
    {
        [SerializeField] private float _doubleClickDeltaTime;
        
        private UnitSelecting _selecting;
        private SelectionInput _selectionInput;
        private SelectionArea _selectionArea;

        private float _lastClickTime;

        public IEnumerable<UnitFacade> Selected { get; private set; } = Array.Empty<UnitFacade>();
        
        [Inject]
        public void Construct(UnitSelecting selecting, SelectionInput selectionInput, SelectionArea selectionArea)
        {
            _selecting = selecting;
            _selectionInput = selectionInput;
            _selectionArea = selectionArea;
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
            Selected = Array.Empty<UnitFacade>();
        }

        private void Draw(Rect rect)
        {
            _selectionArea.Draw(rect);
        }

        private void Select(Rect rect)
        {
            var newSelected = GetSelected(rect);

            var newSelectedArray = newSelected as UnitFacade[] ?? newSelected.ToArray();
            foreach (var willDeselect in Selected.Except(newSelectedArray))
            {
                willDeselect.OnDeselect();
            }

            foreach (var selected in newSelectedArray)
            {
                selected.OnSelect();
            }

            Selected = newSelectedArray;

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
            if (Selected.Count() != 1)
            {
                throw new InvalidOperationException("After the second click must be one selected unit");
            }
            
            return _selecting.SelectByType(Selected.First().UnitType);
        }

        private bool WasDoubleClick()
        {
            return Time.time - _lastClickTime < _doubleClickDeltaTime;
        }
    }
}