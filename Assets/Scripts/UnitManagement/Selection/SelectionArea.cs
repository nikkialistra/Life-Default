using Saving;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnitManagement.Selection
{
    [RequireComponent(typeof(UIDocument))]
    public class SelectionArea : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private VisualElement _root;
        private VisualElement _selection;
        private float _scale;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;
            CalculateScale();

            _selection = _root.Q<VisualElement>("selection");
        }

        private void Start()
        {
            GameSettings.Instance.Resolution.Subscribe(_ => CalculateScale());
        }

#if UNITY_EDITOR

        private void Update()
        {
            _scale = (float)_uiDocument.panelSettings.referenceResolution.x / Screen.width;
        }

#endif

        private void CalculateScale()
        {
            _scale = (float)_uiDocument.panelSettings.referenceResolution.x / Screen.width;
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
