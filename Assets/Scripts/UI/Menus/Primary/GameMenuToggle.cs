using System;
using Game;
using Saving;
using UI.Game.GameLook.Components;
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

        private MenuPanelView _menuPanelView;

        private TimeToggling _timeToggling;
        private GameSettings _gameSettings;

        private PlayerInput _playerInput;

        private InputAction _showMenuAction;
        private InputAction _hideMenuAction;

        [Inject]
        public void Construct(TimeToggling timeToggling, MenuPanelView menuPanelView, GameSettings gameSettings,
            PlayerInput playerInput)
        {
            _timeToggling = timeToggling;
            _menuPanelView = menuPanelView;
            _gameSettings = gameSettings;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("root");

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

            _menuPanelView.Click += ShowGameMenu;
        }

        private void OnDisable()
        {
            _gameMenuView.Pausing += DoPausing;
            _gameMenuView.Resuming -= DoResuming;

            _showMenuAction.started -= ShowMenu;
            _hideMenuAction.started -= HideMenu;

            _menuPanelView.Click -= ShowGameMenu;
        }

        private void DoPausing()
        {
            Pausing?.Invoke();
        }

        private void DoResuming()
        {
            _playerInput.SwitchCurrentActionMap("Management");
            _timeToggling.Toggle();
            Resuming?.Invoke();
        }

        private void ShowMenu(InputAction.CallbackContext context)
        {
            if (_gameMenuView.ShownSubView)
            {
                return;
            }

            ShowGameMenu();
        }

        private void ShowGameMenu()
        {
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
