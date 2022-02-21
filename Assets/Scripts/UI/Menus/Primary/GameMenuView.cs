using System;
using Saving;
using UI.Menus.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.Menus.Primary
{
    public class GameMenuView : IMenuView
    {
        private readonly VisualElement _root;
        private readonly IHideNotify _hideNotify;

        private readonly TemplateContainer _tree;

        private readonly Button _resume;
        private readonly Button _save;
        private readonly Button _saveAs;
        private readonly Button _load;
        private readonly Button _settings;
        private readonly Button _mainMenu;
        private readonly Button _exitGame;

        private SettingsView _settingsView;

        private GameSettings _gameSettings;

        public bool Shown { get; private set; }
        public bool ShownSubView { get; private set; }

        public GameMenuView(VisualElement root, IHideNotify hideNotify, GameSettings gameSettings)
        {
            _root = root;
            _hideNotify = hideNotify;
            _gameSettings = gameSettings;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/GameMenu");
            _tree = template.CloneTree();
            _tree.style.flexGrow = 1;

            _resume = _tree.Q<Button>("resume");
            _save = _tree.Q<Button>("save");
            _saveAs = _tree.Q<Button>("save-as");
            _load = _tree.Q<Button>("load");
            _settings = _tree.Q<Button>("settings");
            _mainMenu = _tree.Q<Button>("main-menu");
            _exitGame = _tree.Q<Button>("exit-game");
        }

        public event Action Pausing;
        public event Action Resuming;

        public void ShowSelf()
        {
            _hideNotify.HideCurrentMenu += Resume;

            Shown = true;
            ShownSubView = false;

            _root.Add(_tree);
            Time.timeScale = 0;

            _resume.clicked += Resume;
            _settings.clicked += Settings;
            _mainMenu.clicked += MainMenu;
            _exitGame.clicked += ExitGame;

            Pausing?.Invoke();
        }

        public void HideSelf()
        {
            _hideNotify.HideCurrentMenu -= Resume;

            Shown = false;
            _root.Remove(_tree);

            _resume.clicked -= Resume;
            _settings.clicked -= Settings;
            _mainMenu.clicked -= MainMenu;
            _exitGame.clicked -= ExitGame;
        }

        private void Resume()
        {
            HideSelf();
            Time.timeScale = 1;
            Resuming?.Invoke();
        }

        private void Settings()
        {
            ShownSubView = true;
            HideSelf();

            _settingsView ??= new SettingsView(_root, this, _hideNotify, _gameSettings);
            _settingsView.ShowSelf();
        }

        private static void MainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        private static void ExitGame()
        {
            Application.Quit();
        }
    }
}
