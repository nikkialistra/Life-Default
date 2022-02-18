using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class GraphicsView : IMenuView
    {
        private readonly VisualElement _root;
        private readonly SettingsView _parent;

        private readonly TemplateContainer _tree;

        private readonly Button _back;

        public GraphicsView(VisualElement root, SettingsView parent)
        {
            _root = root;
            _parent = parent;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/Settings/Graphics");
            _tree = template.CloneTree();

            _back = _tree.Q<Button>("back");
        }

        public bool Shown { get; private set; }

        public void ShowSelf()
        {
            _root.Add(_tree);
            Shown = true;

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _root.Remove(_tree);
            Shown = false;

            _back.clicked -= Back;

            _parent.ShowSelf();
        }

        private void Back()
        {
            HideSelf();
        }
    }
}
