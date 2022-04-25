using System.Collections.Generic;
using System.Linq;
using Colonists;
using Colonists.Services.Selecting;
using Entities.Services;
using General.Selection.Selected;
using UnityEngine;
using Zenject;

namespace General.Selection
{
    [RequireComponent(typeof(SelectionDisplaying))]
    public class SelectionOperation : MonoBehaviour
    {
        private SelectionDisplaying _selectionDisplaying;

        private ObjectSelecting _selecting;
        private SelectionInput _selectionInput;
        private SelectedColonists _selectedColonists;

        private float _lastClickTime;

        private bool _cancelSelection;
        
        private EntitiesSelecting _entitiesSelecting;

        [Inject]
        public void Construct(ObjectSelecting selecting, SelectionInput selectionInput,
            SelectedColonists selectedColonists, EntitiesSelecting entitiesSelecting)
        {
            _selectedColonists = selectedColonists;
            _selecting = selecting;
            _selectionInput = selectionInput;
            _entitiesSelecting = entitiesSelecting;
        }

        public void CancelSelection()
        {
            _cancelSelection = true;
        }

        private void Awake()
        {
            _selectionDisplaying = GetComponent<SelectionDisplaying>();
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
                _selectionDisplaying.Draw(rect);
            }
        }

        private void Select(Rect rect)
        {
            _selectionDisplaying.StopDrawing();
            
            if (_cancelSelection)
            {
                _cancelSelection = false;
                return;
            }
            
            var newSelected = GetSelected(rect).ToList();

            if (newSelected.Count > 0)
            {
                _entitiesSelecting.DeselectEntity();
            }
            
            _selectedColonists.Set(newSelected);
        }

        private IEnumerable<Colonist> GetSelected(Rect rect)
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

        private IEnumerable<Colonist> GetSelectedFromClick(Rect rect)
        {
            return _selecting.SelectFromPoint(rect.center);
        }

        private static bool WasClick(Rect rect)
        {
            return rect.width <= 8f && rect.height <= 8f;
        }
    }
}
