using UnityEngine;
using Zenject;

namespace General.Selecting
{
    [RequireComponent(typeof(SelectingAreaDisplaying))]
    public class SelectingOperation : MonoBehaviour
    {
        private SelectingAreaDisplaying _selectingAreaDisplaying;

        private Selection _selection;
        private SelectingInput _selectingInput;

        private float _lastClickTime;

        private bool _cancelSelecting;

        [Inject]
        public void Construct(Selection selection, SelectingInput selectingInput)
        {
            _selection = selection;
            _selectingInput = selectingInput;
        }

        public void CancelSelecting()
        {
            _cancelSelecting = true;
        }

        private void Awake()
        {
            _selectingAreaDisplaying = GetComponent<SelectingAreaDisplaying>();
        }

        public void OnEnable()
        {
            _selectingInput.Selecting += Draw;
            _selectingInput.SelectingEnd += Select;
        }

        public void OnDisable()
        {
            _selectingInput.Selecting -= Draw;
            _selectingInput.SelectingEnd -= Select;
        }

        private void Draw(Rect rect)
        {
            if (!WasClick(rect))
            {
                _selectingAreaDisplaying.Draw(rect);
            }
        }

        private void Select(Rect rect)
        {
            _selectingAreaDisplaying.StopDrawing();
            
            if (_cancelSelecting)
            {
                _cancelSelecting = false;
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
                _selection.SelectFromRect(rect);
            }
        }

        private void SelectFromClick(Rect rect)
        {
            _selection.SelectFromPoint(rect.center);
        }

        private static bool WasClick(Rect rect)
        {
            return rect.width <= 8f && rect.height <= 8f;
        }
    }
}
