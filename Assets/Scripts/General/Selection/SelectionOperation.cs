using UnityEngine;
using Zenject;

namespace General.Selection
{
    [RequireComponent(typeof(SelectionDisplaying))]
    public class SelectionOperation : MonoBehaviour
    {
        private SelectionDisplaying _selectionDisplaying;

        private Selecting _selecting;
        private SelectionInput _selectionInput;

        private float _lastClickTime;

        private bool _cancelSelection;

        [Inject]
        public void Construct(Selecting selecting, SelectionInput selectionInput)
        {
            _selecting = selecting;
            _selectionInput = selectionInput;
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

            MakeSelection(rect);
        }

        private void MakeSelection(Rect rect)
        {
            if (WasClick(rect))
            {
                SelectFromClick(rect);
            }
            else
            {
                _selecting.SelectFromRect(rect);
            }
        }

        private void SelectFromClick(Rect rect)
        {
            _selecting.SelectFromPoint(rect.center);
        }

        private static bool WasClick(Rect rect)
        {
            return rect.width <= 8f && rect.height <= 8f;
        }
    }
}
