using System;
using Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Menus.Primary
{
    [RequireComponent(typeof(UIDocument))]
    public class GameMenuToggle : MonoBehaviour, IHideNotify
    {
        private VisualElement _root;

        private GameMenuView _gameMenuView;

        private PlayerInput _playerInput;

        private InputAction _showMenuAction;
        private InputAction _hideMenuAction;

        private GameSettings _gameSettings;

        [Inject]
        public void Construct(PlayerInput playerInput, GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _gameMenuView = new GameMenuView(_root, this, _gameSettings);

            _showMenuAction = _playerInput.actions.FindAction("Show Menu");
            _hideMenuAction = _playerInput.actions.FindAction("Hide Menu");
        }

        public event Action HideCurrentMenu;

        public event Action Pausing;

        public event Action Resuming;

        private void Start()
        {
            Time.timeScale = 1;
            _playerInput.SwitchCurrentActionMap("Management");
        }

        private void OnEnable()
        {
            _gameMenuView.Pausing += DoPausing;
            _gameMenuView.Resuming += DoResuming;

            _showMenuAction.started += ShowMenu;
            _hideMenuAction.started += HideMenu;
        }

        private void OnDisable()
        {
            _gameMenuView.Pausing += DoPausing;
            _gameMenuView.Resuming -= DoResuming;

            _showMenuAction.started -= ShowMenu;
            _hideMenuAction.started -= HideMenu;
        }

        private void DoPausing()
        {
            Pausing?.Invoke();
        }

        private void DoResuming()
        {
            _playerInput.SwitchCurrentActionMap("Management");
            Resuming?.Invoke();
        }

        private void ShowMenu(InputAction.CallbackContext context)
        {
            if (_gameMenuView.ShownSubView)
            {
                return;
            }

            _gameMenuView.ShowSelf();
            _playerInput.SwitchCurrentActionMap("Menus");
        }

        private void HideMenu(InputAction.CallbackContext context)
        {
            if (_gameMenuView.Shown)
            {
                _playerInput.SwitchCurrentActionMap("Management");
            }

            HideCurrentMenu?.Invoke();
        }
    }
}
