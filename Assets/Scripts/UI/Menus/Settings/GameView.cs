using Saving;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class GameView : IMenuView
    {
        private const string VisualTreePath = "UI/Markup/Menus/Settings/Game";

        private readonly VisualElement _root;
        private readonly SettingsView _parent;

        private readonly TemplateContainer _tree;

        private readonly Toggle _showHelpPanelAtStart;
        private readonly Slider _cameraSensitivity;
        private readonly Toggle _screenEdgeMouseScroll;

        private readonly Button _back;

        private readonly IHideNotify _hideNotify;

        private readonly GameSettings _gameSettings;

        public GameView(VisualElement root, SettingsView parent, IHideNotify hideNotify, GameSettings gameSettings)
        {
            _root = root;
            _parent = parent;
            _hideNotify = hideNotify;
            _gameSettings = gameSettings;

            var template = Resources.Load<VisualTreeAsset>(VisualTreePath);
            _tree = template.CloneTree();
            _tree.style.flexGrow = 1;

            _showHelpPanelAtStart = _tree.Q<Toggle>("show-help-panel-at-start");
            _cameraSensitivity = _tree.Q<Slider>("camera-sensitivity");
            _screenEdgeMouseScroll = _tree.Q<Toggle>("screen-edge-mouse-scroll");

            _back = _tree.Q<Button>("back");

            UpdateParameters();
        }

        public void ShowSelf()
        {
            _hideNotify.HideCurrentMenu += Back;

            _root.Add(_tree);

            _showHelpPanelAtStart.RegisterValueChangedCallback(OnShowHelpToggle);
            _cameraSensitivity.RegisterValueChangedCallback(OnCameraSensitivityChanged);
            _screenEdgeMouseScroll.RegisterValueChangedCallback(OnScreenEdgeToggle);

            _back.clicked += Back;
        }

        public void HideSelf()
        {
            _hideNotify.HideCurrentMenu -= Back;

            _root.Remove(_tree);

            _showHelpPanelAtStart.UnregisterValueChangedCallback(OnShowHelpToggle);
            _cameraSensitivity.UnregisterValueChangedCallback(OnCameraSensitivityChanged);
            _screenEdgeMouseScroll.UnregisterValueChangedCallback(OnScreenEdgeToggle);

            _back.clicked -= Back;

            _parent.ShowSelf();
        }

        private void OnShowHelpToggle(ChangeEvent<bool> _)
        {
            _gameSettings.ShowHelpPanelAtStart = !_showHelpPanelAtStart.value;
        }

        private void OnCameraSensitivityChanged(ChangeEvent<float> _)
        {
            _gameSettings.CameraSensitivity.Value = _cameraSensitivity.value;
        }

        private void OnScreenEdgeToggle(ChangeEvent<bool> _)
        {
            _gameSettings.ScreenEdgeMouseScroll.Value = _screenEdgeMouseScroll.value;
        }

        private void UpdateParameters()
        {
            _showHelpPanelAtStart.value = !_gameSettings.ShowHelpPanelAtStart;
            _cameraSensitivity.value = _gameSettings.CameraSensitivity.Value;
            _screenEdgeMouseScroll.value = _gameSettings.ScreenEdgeMouseScroll.Value;
        }

        private void Back()
        {
            HideSelf();
        }
    }
}
