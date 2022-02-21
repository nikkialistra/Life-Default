using System.Collections;
using Saving;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UnitManagement.Selection
{
    [RequireComponent(typeof(UIDocument))]
    public class SelectionArea : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private VisualElement _root;
        private VisualElement _selection;

        private float _width;
        private float _height;

        private GameSettings _gameSettings;

        [Inject]
        public void Construct(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;
            UpdateSize();

            _selection = _root.Q<VisualElement>("selection");
        }

        private void Start()
        {
            _gameSettings.Resolution.Subscribe(_ => UpdateSize());
        }

#if UNITY_EDITOR

        private void Update()
        {
            _width = Screen.width;
            _height = Screen.height;
        }

#endif

        private void UpdateSize()
        {
            StartCoroutine(UpdateSizeAtNextFrame());
        }

        private IEnumerator UpdateSizeAtNextFrame()
        {
            yield return null;

            _width = Screen.width;
            _height = Screen.height;
        }

        public void Draw(Rect rect)
        {
            if (_selection.ClassListContains("not-displayed"))
            {
                _selection.RemoveFromClassList("not-displayed");
            }

            _selection.style.left = new Length(rect.xMin / _width * 100f, LengthUnit.Percent);
            _selection.style.bottom = new Length(rect.yMin / _height * 100f, LengthUnit.Percent);
            _selection.style.width = new Length(rect.width / _width * 100f, LengthUnit.Percent);
            _selection.style.height = new Length(rect.height / _height * 100f, LengthUnit.Percent);
        }

        public void StopDrawing()
        {
            _selection.AddToClassList("not-displayed");
        }
    }
}
