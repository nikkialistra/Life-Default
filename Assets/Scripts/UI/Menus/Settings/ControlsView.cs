using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class ControlsView : IMenuView
    {
        private readonly VisualElement _root;
        private readonly SettingsView _parent;

        private readonly TemplateContainer _tree;

        private readonly Button _back;
        private readonly MenuViews _menuViews;

        public ControlsView(VisualElement root, SettingsView parent, MenuViews menuViews)
        {
            _root = root;
            _parent = parent;
            _menuViews = menuViews;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/Settings/Controls");
            _tree = template.CloneTree();

            _back = _tree.Q<Button>("back");
        }

        public void ShowSelf()
        {
            _menuViews.HideCurrentMenu += Back;

            _root.Add(_tree);

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _menuViews.HideCurrentMenu -= Back;

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
