using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class SettingsView : IMenuView
    {
        private readonly VisualElement _root;
        private readonly GameMenuView _parent;

        private readonly TemplateContainer _tree;

        private readonly Button _graphics;
        private readonly Button _audio;
        private readonly Button _game;
        private readonly Button _controls;
        private readonly Button _language;
        private readonly Button _back;

        private GraphicsView _graphicsView;

        public SettingsView(VisualElement root, GameMenuView parent)
        {
            _root = root;
            _parent = parent;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/Settings/Settings");
            _tree = template.CloneTree();

            _graphics = _tree.Q<Button>("graphics");
            _audio = _tree.Q<Button>("audio");
            _game = _tree.Q<Button>("game");
            _controls = _tree.Q<Button>("controls");
            _language = _tree.Q<Button>("language");
            _back = _tree.Q<Button>("back");
        }

        public bool Shown { get; private set; }

        public void ShowSelf()
        {
            _root.Add(_tree);
            Shown = true;

            _graphics.clicked += Graphics;
            _audio.clicked += Audio;
            _game.clicked += Game;
            _controls.clicked += Controls;
            _language.clicked += Language;

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _root.Remove(_tree);
            Shown = false;

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

            _graphicsView ??= new GraphicsView(_root, this);
            _graphicsView.ShowSelf();
        }

        private void Audio() { }

        private void Game() { }

        private void Controls() { }

        private void Language() { }

        private void Back()
        {
            HideSelf();
            _parent.ShowSelf();
        }
    }
}
