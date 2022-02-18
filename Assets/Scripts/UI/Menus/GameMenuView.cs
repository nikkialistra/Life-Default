using System;
using UI.Menus.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.Menus
{
    public class GameMenuView : IMenuView
    {
        private readonly VisualElement _root;

        private readonly TemplateContainer _tree;

        private readonly Button _resume;
        private readonly Button _save;
        private readonly Button _saveAs;
        private readonly Button _load;
        private readonly Button _settings;
        private readonly Button _mainMenu;
        private readonly Button _exitGame;

        private SettingsView _settingsView;

        public GameMenuView(VisualElement root)
        {
            _root = root;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/GameMenu");
            _tree = template.CloneTree();

            _resume = _tree.Q<Button>("resume");
            _save = _tree.Q<Button>("save");
            _saveAs = _tree.Q<Button>("save-as");
            _load = _tree.Q<Button>("load");
            _settings = _tree.Q<Button>("settings");
            _mainMenu = _tree.Q<Button>("main-menu");
            _exitGame = _tree.Q<Button>("exit-game");
        }

        public event Action Resuming;

        public bool Shown { get; private set; }

        public void ShowSelf()
        {
            _root.Add(_tree);
            Shown = true;
            Time.timeScale = 0;

            _resume.clicked += Resume;
            _settings.clicked += Settings;
            _mainMenu.clicked += MainMenu;
            _exitGame.clicked += ExitGame;
        }

        public void HideSelf()
        {
            _root.Remove(_tree);
            Shown = false;

            _resume.clicked -= Resume;
            _settings.clicked -= Settings;
            _mainMenu.clicked -= MainMenu;
            _exitGame.clicked -= ExitGame;
        }

        private void Resume()
        {
            Resuming?.Invoke();
            Time.timeScale = 1;
            HideSelf();
        }

        private void Settings()
        {
            HideSelf();

            _settingsView ??= new SettingsView(_root, this);
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
