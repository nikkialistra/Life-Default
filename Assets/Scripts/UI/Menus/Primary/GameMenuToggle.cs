using System;
using Cameras;
using Environment.Time;
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
        private CameraMovement _cameraMovement;
        private GameSettings _gameSettings;

        private PlayerInput _playerInput;

        private InputAction _toggleMenuAction;

        [Inject]
        public void Construct(TimeToggling timeToggling, CameraMovement cameraMovement, MenuPanelView menuPanelView,
            GameSettings gameSettings,
            PlayerInput playerInput)
        {
            _timeToggling = timeToggling;
            _cameraMovement = cameraMovement;
            _menuPanelView = menuPanelView;
            _gameSettings = gameSettings;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("root");

            _gameMenuView = new GameMenuView(_root, this, _gameSettings);

            _toggleMenuAction = _playerInput.actions.FindAction("Toggle Menu");
        }

        public event Action HideCurrentMenu;

        public event Action Pausing;
        public event Action Resuming;

        private void Start()
        {
            Time.timeScale = 1;
        }

        private void OnEnable()
        {
            _gameMenuView.Pausing += DoPausing;
            _gameMenuView.Resuming += DoResuming;

            _toggleMenuAction.started += ToggleMenu;

            _menuPanelView.Click += ShowGameMenu;
        }

        private void OnDisable()
        {
            _gameMenuView.Pausing += DoPausing;
            _gameMenuView.Resuming -= DoResuming;

            _toggleMenuAction.started -= ToggleMenu;

            _menuPanelView.Click -= ShowGameMenu;
        }

        private void DoPausing()
        {
            _cameraMovement.DeactivateMovement();
            Pausing?.Invoke();
        }

        private void DoResuming()
        {
            _timeToggling.ToggleTime();
            _cameraMovement.ActivateMovement();
            Resuming?.Invoke();
        }

        private void ShowGameMenu()
        {
            _gameMenuView.ShowSelf();
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (_gameMenuView.ShownSubView)
            {
                HideCurrentMenu?.Invoke();
            }
            else
            {
                ToggleGameMenu();
            }
        }

        private void ToggleGameMenu()
        {
            if (_gameMenuView.Shown)
            {
                _gameMenuView.HideSelf();
            }
            else
            {
                _gameMenuView.ShowSelf();
            }
        }
    }
}
