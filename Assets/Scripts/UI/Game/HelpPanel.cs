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

            _toggleHelpPage = _playerInput.actions.FindAction("Toggle Help Panel");
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey("ShownAtFirstStart"))
            {
                _helpPanel.AddToClassList("not-displayed");
            }
            else
            {
                PlayerPrefs.SetInt("ShownAtFirstStart", 1);
            }
        }

        private void OnEnable()
        {
            _toggleHelpPage.started += ToggleHelpPage;
        }

        private void OnDisable()
        {
            _toggleHelpPage.started -= ToggleHelpPage;
        }

        private void ToggleHelpPage(InputAction.CallbackContext context)
        {
            _helpPanel.ToggleInClassList("not-displayed");
        }
    }
}
