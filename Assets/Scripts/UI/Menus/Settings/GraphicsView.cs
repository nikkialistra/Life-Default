using System.Collections.Generic;
using System.Linq;
using Saving;
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
        private readonly SliderInt _uiScale;
        private readonly Label _uiScaleLabel;
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
            _uiScale = _tree.Q<SliderInt>("ui-scale");
            _uiScaleLabel = (Label)_uiScale.Children().First();

            _back = _tree.Q<Button>("back");

            UpdateFullscreenToggle();
            FillResolutions();
            UpdateUiScale();
        }

        public void ShowSelf()
        {
            _menuViews.HideCurrentMenu += Back;

            _root.Add(_tree);

            _fullscreen.RegisterValueChangedCallback(OnFullscreenToggle);
            _resolution.RegisterValueChangedCallback(OnResolutionChange);
            _uiScale.RegisterValueChangedCallback(OnUiScaleChange);

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _menuViews.HideCurrentMenu -= Back;

            _root.Remove(_tree);

            _fullscreen.UnregisterValueChangedCallback(OnFullscreenToggle);
            _resolution.UnregisterValueChangedCallback(OnResolutionChange);

            _back.clicked -= Back;

            _parent.ShowSelf();
        }

        private void OnFullscreenToggle(ChangeEvent<bool> _)
        {
            UpdateFullscreenToggle();

            Screen.fullScreen = _fullscreen.value;

            GameSettings.Instance.Fullscreen = _fullscreen.value;
        }

        private void OnResolutionChange(ChangeEvent<string> _)
        {
            var index = _resolution.index;
            Screen.SetResolution(_resolutions[index].width, _resolutions[index].height, _fullscreen.value);

            GameSettings.Instance.Resolution = _resolutions[index].ToString();
        }

        private void OnUiScaleChange(ChangeEvent<int> _)
        {
            SetUiScaleLabelText();

            var value = RoundScale(_uiScale.value);
            GameSettings.Instance.UiScale = value;
        }

        private void SetUiScaleLabelText()
        {
            var value = RoundScale(_uiScale.value);
            _uiScaleLabel.text = "Scale (" + value + "):";
        }

        private static int RoundScale(int scale)
        {
            var roundedBy5Value = scale / 5;
            var value = roundedBy5Value * 5;
            return value;
        }

        private void UpdateFullscreenToggle()
        {
            GameSettings.Instance.Fullscreen = _fullscreen.value;

            if (_fullscreen.value)
            {
                _fullscreen.AddToClassList("selected");
            }
            else
            {
                _fullscreen.RemoveFromClassList("selected");
            }
        }

        private void FillResolutions()
        {
            _resolution.choices = new List<string>();
            var currentIndex = 0;
            for (var i = 0; i < _resolutions.Length; i++)
            {
                var resolution = _resolutions[i];
                var resolutionText = resolution.ToString();
                _resolution.choices.Add(resolutionText);

                if (resolutionText == GameSettings.Instance.Resolution)
                {
                    currentIndex = i;
                }
            }

            _resolution.index = currentIndex;
        }

        private void UpdateUiScale()
        {
            _uiScale.value = GameSettings.Instance.UiScale;
            SetUiScaleLabelText();
        }

        private void Back()
        {
            HideSelf();
        }
    }
}
