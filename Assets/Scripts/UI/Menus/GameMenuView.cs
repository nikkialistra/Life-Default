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
        private readonly MenuViews _menuViews;

        private readonly TemplateContainer _tree;

        private readonly Button _resume;
        private readonly Button _save;
        private readonly Button _saveAs;
        private readonly Button _load;
        private readonly Button _settings;
        private readonly Button _mainMenu;
        private readonly Button _exitGame;

        private SettingsView _settingsView;

        public bool Shown { get; private set; }

        public GameMenuView(VisualElement root, MenuViews menuViews)
        {
            _root = root;
            _menuViews = menuViews;

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

        public void ShowSelf()
        {
            _menuViews.HideCurrentMenu += HideSelf;

            Shown = true;
            _root.Add(_tree);
            Time.timeScale = 0;

            _resume.clicked += Resume;
            _settings.clicked += Settings;
            _mainMenu.clicked += MainMenu;
            _exitGame.clicked += ExitGame;
        }

        public void HideSelf()
        {
            _menuViews.HideCurrentMenu -= HideSelf;

            Shown = false;
            _root.Remove(_tree);

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

            _settingsView ??= new SettingsView(_root, this, _menuViews);
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
