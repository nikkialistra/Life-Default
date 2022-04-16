﻿using System.Collections.Generic;
using System.Linq;
using ColonistManagement.Selection;
using Colonists.Colonist;
using UnityEngine;
using Zenject;

namespace Colonists.Services.Selecting
{
    [RequireComponent(typeof(SelectionArea))]
    public class ColonistSelection : MonoBehaviour
    {
        private SelectionArea _selectionArea;

        private ColonistSelecting _selecting;
        private ColonistSelectionInput _colonistSelectionInput;
        private SelectedColonists _selectedColonists;

        private float _lastClickTime;

        private bool _cancelSelection;

        [Inject]
        public void Construct(ColonistSelecting selecting, ColonistSelectionInput colonistSelectionInput, SelectedColonists selectedColonists)
        {
            _selectedColonists = selectedColonists;
            _selecting = selecting;
            _colonistSelectionInput = colonistSelectionInput;
        }

        public void CancelSelection()
        {
            _cancelSelection = true;
        }

        private void Awake()
        {
            _selectionArea = GetComponent<SelectionArea>();
        }

        public void OnEnable()
        {
            _colonistSelectionInput.Selecting += Draw;
            _colonistSelectionInput.SelectingEnd += Select;
        }

        public void OnDisable()
        {
            _colonistSelectionInput.Selecting -= Draw;
            _colonistSelectionInput.SelectingEnd -= Select;
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
            _selectionArea.StopDrawing();
            
            if (_cancelSelection)
            {
                _cancelSelection = false;
                return;
            }
            
            var newSelected = GetSelected(rect).ToList();

            _selectedColonists.Set(newSelected);
        }

        private IEnumerable<ColonistFacade> GetSelected(Rect rect)
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

        private IEnumerable<ColonistFacade> GetSelectedFromClick(Rect rect)
        {
            return _selecting.SelectFromPoint(rect.center);
        }

        private static bool WasClick(Rect rect)
        {
            return (rect.width <= 8f && rect.height <= 8f);
        }
    }
}
