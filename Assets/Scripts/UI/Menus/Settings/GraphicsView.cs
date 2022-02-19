using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class GraphicsView : IMenuView
    {
        private readonly Resolution[] _resolutions = Screen.resolutions;

        private readonly VisualElement _root;
        private readonly SettingsView _parent;

        private readonly TemplateContainer _tree;

        private readonly Toggle _fullscreen;
        private readonly DropdownField _resolution;
        private readonly DropdownField _uiScale;
        private readonly Button _back;

        private PlayerInput _playerInput;
        private readonly MenuViews _menuViews;

        public GraphicsView(VisualElement root, SettingsView parent, MenuViews menuViews)
        {
            _root = root;
            _parent = parent;
            _menuViews = menuViews;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/Settings/Graphics");
            _tree = template.CloneTree();

            _fullscreen = _tree.Q<Toggle>("fullscreen");
            _resolution = _tree.Q<DropdownField>("resolution");
            _uiScale = _tree.Q<DropdownField>("ui-scale");
            _back = _tree.Q<Button>("back");

            FillResolutions();
        }

        public void ShowSelf()
        {
            _menuViews.HideCurrentMenu += Back;

            _root.Add(_tree);

            _fullscreen.RegisterCallback<ChangeEvent<bool>>(OnFullscreenToggle);

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _menuViews.HideCurrentMenu -= Back;

            _root.Remove(_tree);

            _fullscreen.UnregisterCallback<ChangeEvent<bool>>(OnFullscreenToggle);

            _back.clicked -= Back;

            _parent.ShowSelf();
        }

        private void OnFullscreenToggle(ChangeEvent<bool> changeEvent)
        {
            if (changeEvent.newValue)
            {
                _fullscreen.AddToClassList("selected");
            }
            else
            {
                _fullscreen.RemoveFromClassList("selected");
            }

            Screen.fullScreen = changeEvent.newValue;
        }

        private void FillResolutions()
        {
            _resolution.choices = new List<string>();
            foreach (var resolution in _resolutions)
            {
                var resolutionText = resolution.ToString();
                _resolution.choices.Add(resolutionText);
            }
        }

        private void Back()
        {
            HideSelf();
        }
    }
}
