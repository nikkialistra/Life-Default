using System;
using Controls;
using Controls.CameraControls;
using General;
using General.TimeCycle.TimeRegulation;
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
        public event Action HideCurrentMenu;

        public event Action GamePause;
        public event Action GameResume;

        private VisualElement _root;

        private GameMenuView _gameMenuView;

        private TimeToggling _timeToggling;
        private CameraMovement _cameraMovement;
        private GameSettings _gameSettings;

        private PlayerInput _playerInput;

        private InputAction _toggleMenuAction;

        [Inject]
        public void Construct(TimeToggling timeToggling, CameraMovement cameraMovement,
            GameSettings gameSettings,
            PlayerInput playerInput)
        {
            _timeToggling = timeToggling;
            _cameraMovement = cameraMovement;
            _gameSettings = gameSettings;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("root");

            _gameMenuView = new GameMenuView(_root, this, _gameSettings);

            _toggleMenuAction = _playerInput.actions.FindAction("Toggle Menu");
        }

        private void Start()
        {
            Time.timeScale = 1;
        }

        private void OnEnable()
        {
            _gameMenuView.Pause += DoPause;
            _gameMenuView.Resume += DoResume;

            _toggleMenuAction.started += ToggleMenu;
        }

        private void OnDisable()
        {
            _gameMenuView.Pause += DoPause;
            _gameMenuView.Resume -= DoResume;

            _toggleMenuAction.started -= ToggleMenu;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                DoResume();
            else
                DoPause();
        }

        private void DoPause()
        {
            _cameraMovement.DeactivateMovement();
            GamePause?.Invoke();
        }

        private void DoResume()
        {
            _timeToggling.ToggleTime();
            _cameraMovement.ActivateMovement();
            GameResume?.Invoke();
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (_gameMenuView.ShownSubView)
                HideCurrentMenu?.Invoke();
            else
                ToggleGameMenu();
        }

        private void ToggleGameMenu()
        {
            if (_gameMenuView.Shown)
                _gameMenuView.HideSelf();
            else
                _gameMenuView.ShowSelf();
        }
    }
}
