using UnityEngine;
using UnityEngine.UIElements;

namespace UnitManagement.Selection
{
    [RequireComponent(typeof(UIDocument))]
    public class SelectionArea : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _selection;
        private float _scale;

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;
            _scale = (float)uiDocument.panelSettings.referenceResolution.x / Screen.width;

            _selection = _root.Q<VisualElement>("selection");
        }

        public void Draw(Rect rect)
        {
            if (_selection.ClassListContains("not-displayed"))
            {
                _selection.RemoveFromClassList("not-displayed");
            }

            _selection.style.left = rect.xMin * _scale;
            _selection.style.bottom = rect.yMin * _scale;
            _selection.style.width = rect.width * _scale;
            _selection.style.height = rect.height * _scale;
        }

        public void StopDrawing()
        {
            _selection.AddToClassList("not-displayed");
        }
    }
}