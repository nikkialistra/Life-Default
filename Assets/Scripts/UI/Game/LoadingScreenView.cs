using General;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;
using Cursor = UnityEngine.Cursor;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class LoadingScreenView : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _loadingScreen;

        private Map _map;

        [Inject]
        public void Construct(Map map)
        {
            _map = map;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _loadingScreen = _root.Q<VisualElement>("loading-screen");
        }

        private void Start()
        {
            HideCursor();
        }

        private void OnEnable()
        {
            _map.Load += Hide;
        }

        private void OnDisable()
        {
            _map.Load -= Hide;
        }

        private void Hide()
        {
            _loadingScreen.AddToClassList("not-displayed");
            ShowCursor();
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void ShowCursor()
        {
            Cursor.visible = true;
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
