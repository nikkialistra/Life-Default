using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace General.Selecting
{
    public class SelectingOperation : MonoBehaviour
    {
        [Required]
        [SerializeField] private SelectingAreaDisplaying _selectingAreaDisplaying;
        [Space]
        [SerializeField] private float _doubleClickDeltaTime = 0.15f;

        private Selection _selection;
        private SelectingInput _selectingInput;

        private float _lastClickTime;

        [Inject]
        public void Construct(Selection selection, SelectingInput selectingInput)
        {
            _selection = selection;
            _selectingInput = selectingInput;
        }

        public void CancelSelecting()
        {
            _selection.CancelSelecting();
        }

        public void OnEnable()
        {
            _selectingInput.Selecting += Draw;
            _selectingInput.SelectingEnd += Select;
            _selectingInput.SelectingEndAdditive += SelectAdditive;
        }

        public void OnDisable()
        {
            _selectingInput.Selecting -= Draw;
            _selectingInput.SelectingEnd -= Select;
            _selectingInput.SelectingEndAdditive -= SelectAdditive;
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

            MakeSelection(rect);
        }

        private void SelectAdditive(Rect rect)
        {
            _selectingAreaDisplaying.StopDrawing();

            MakeSelectionAdditive(rect);
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

        private void MakeSelectionAdditive(Rect rect)
        {
            if (WasClick(rect))
            {
                SelectFromClickAdditive(rect);
            }
            else
            {
                _selection.SelectFromRectAdditive(rect);
            }
        }

        private void SelectFromClick(Rect rect)
        {
            if (WasDoubleClick())
            {
                _selection.SelectFromAreaAround();
            }
            else
            {
                _lastClickTime = Time.time;
                _selection.SelectFromPoint(rect.center);
            }
        }

        private void SelectFromClickAdditive(Rect rect)
        {
            if (WasDoubleClick())
            {
                _selection.SelectFromAreaAround();
            }
            else
            {
                _lastClickTime = Time.time;
                _selection.SelectFromPointAdditive(rect.center);
            }
        }

        private static bool WasClick(Rect rect)
        {
            return rect.width <= 8f && rect.height <= 8f;
        }
        
        private bool WasDoubleClick()
        {
            return Time.time - _lastClickTime < _doubleClickDeltaTime;
        }
    }
}
