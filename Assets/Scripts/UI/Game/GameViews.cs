using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class GameViews : MonoBehaviour
    {
        private VisualElement _root;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        public bool MouseOverUi { get; private set; }

        private void OnEnable()
        {
            _root.RegisterCallback<MouseOverEvent, bool>(SetMouseOverUi, true);
            _root.RegisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi, false);
        }

        private void OnDisable()
        {
            _root.UnregisterCallback<MouseOverEvent, bool>(SetMouseOverUi);
            _root.UnregisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi);
        }

        private void SetMouseOverUi(IMouseEvent mouseEvent, bool value)
        {
            MouseOverUi = value;
        }
    }
}
