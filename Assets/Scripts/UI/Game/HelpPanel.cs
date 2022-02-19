using Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class HelpPanel : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _helpPanel;
        private Button _hide;
        private Toggle _dontShowAtStart;

        private PlayerInput _playerInput;

        private InputAction _toggleHelpPage;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _helpPanel = _root.Q<VisualElement>("help-panel");
            _hide = _root.Q<Button>("hide");
            _dontShowAtStart = _root.Q<Toggle>("dont-show-at-start");

            _toggleHelpPage = _playerInput.actions.FindAction("Toggle Help Panel");
        }

        private void Start()
        {
            if (GameSettings.Instance.ShowHelpPanelAtStart)
            {
                _dontShowAtStart.value = false;
            }
            else
            {
                _dontShowAtStart.value = true;
                Hide();
            }
        }

        private void OnEnable()
        {
            _hide.clicked += Hide;
            _toggleHelpPage.started += ToggleHelpPage;
            _dontShowAtStart.RegisterValueChangedCallback(OnShowToggle);
        }

        private void OnDisable()
        {
            _hide.clicked -= Hide;
            _toggleHelpPage.started -= ToggleHelpPage;
            _dontShowAtStart.UnregisterValueChangedCallback(OnShowToggle);
        }

        private void OnShowToggle(ChangeEvent<bool> _)
        {
            GameSettings.Instance.ShowHelpPanelAtStart = !_dontShowAtStart.value;
        }

        private void Hide()
        {
            _helpPanel.AddToClassList("not-displayed");
        }

        private void ToggleHelpPage(InputAction.CallbackContext context)
        {
            _helpPanel.ToggleInClassList("not-displayed");
        }
    }
}
