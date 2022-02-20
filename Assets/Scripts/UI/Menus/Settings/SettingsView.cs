using Saving;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class SettingsView : IMenuView
    {
        private readonly VisualElement _root;
        private readonly IMenuView _parent;

        private readonly TemplateContainer _tree;

        private readonly Button _graphics;
        private readonly Button _audio;
        private readonly Button _game;
        private readonly Button _controls;
        private readonly Button _language;
        private readonly Button _back;

        private GraphicsView _graphicsView;
        private AudioView _audioView;
        private GameView _gameView;
        private ControlsView _controlsView;
        private LanguageView _languageView;
        private readonly IHideNotify _hideNotify;

        private readonly GameSettings _gameSettings;

        public SettingsView(VisualElement root, IMenuView parent, IHideNotify hideNotify, GameSettings gameSettings)
        {
            _root = root;
            _parent = parent;
            _hideNotify = hideNotify;
            _gameSettings = gameSettings;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/Settings/Settings");
            _tree = template.CloneTree();

            _graphics = _tree.Q<Button>("graphics");
            _audio = _tree.Q<Button>("audio");
            _game = _tree.Q<Button>("game");
            _controls = _tree.Q<Button>("controls");
            _language = _tree.Q<Button>("language");
            _back = _tree.Q<Button>("back");
        }

        public void ShowSelf()
        {
            _hideNotify.HideCurrentMenu += Back;

            _root.Add(_tree);

            _graphics.clicked += Graphics;
            _audio.clicked += Audio;
            _game.clicked += Game;
            _controls.clicked += Controls;
            _language.clicked += Language;

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _hideNotify.HideCurrentMenu -= Back;

            _root.Remove(_tree);

            _graphics.clicked -= Graphics;
            _audio.clicked -= Audio;
            _game.clicked -= Game;
            _controls.clicked -= Controls;
            _language.clicked -= Language;

            _back.clicked -= Back;
        }

        private void Graphics()
        {
            HideSelf();

            _graphicsView ??= new GraphicsView(_root, this, _hideNotify, _gameSettings);
            _graphicsView.ShowSelf();
        }

        private void Audio()
        {
            HideSelf();

            _audioView ??= new AudioView(_root, this, _hideNotify, _gameSettings);
            _audioView.ShowSelf();
        }

        private void Game()
        {
            HideSelf();

            _gameView ??= new GameView(_root, this, _hideNotify, _gameSettings);
            _gameView.ShowSelf();
        }

        private void Controls()
        {
            HideSelf();

            _controlsView ??= new ControlsView(_root, this, _hideNotify, _gameSettings);
            _controlsView.ShowSelf();
        }

        private void Language()
        {
            HideSelf();

            _languageView ??= new LanguageView(_root, this, _hideNotify, _gameSettings);
            _languageView.ShowSelf();
        }

        private void Back()
        {
            HideSelf();
            _parent.ShowSelf();
        }
    }
}
