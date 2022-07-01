using Saving;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class AudioView : IMenuView
    {
        private const string VisualTreePath = "UI/Markup/Menus/Settings/Audio";

        private readonly VisualElement _root;
        private readonly SettingsView _parent;

        private readonly TemplateContainer _tree;

        private readonly Button _back;
        private readonly IHideNotify _hideNotify;

        private readonly GameSettings _gameSettings;

        public AudioView(VisualElement root, SettingsView parent, IHideNotify hideNotify, GameSettings gameSettings)
        {
            _root = root;
            _parent = parent;
            _hideNotify = hideNotify;
            _gameSettings = gameSettings;

            var template = Resources.Load<VisualTreeAsset>(VisualTreePath);
            _tree = template.CloneTree();
            _tree.style.flexGrow = 1;

            _back = _tree.Q<Button>("back");
        }

        public void ShowSelf()
        {
            _hideNotify.HideCurrentMenu += Back;

            _root.Add(_tree);

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _hideNotify.HideCurrentMenu -= Back;

            _root.Remove(_tree);

            _back.clicked -= Back;

            _parent.ShowSelf();
        }

        private void Back()
        {
            HideSelf();
        }
    }
}
