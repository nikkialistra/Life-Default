using Saving;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Menus.Settings
{
    public class GameView : IMenuView
    {
        private readonly VisualElement _root;
        private readonly SettingsView _parent;

        private readonly TemplateContainer _tree;

        private readonly Toggle _showHelpPanelAtStart;
        private readonly Slider _cameraSensitivity;
        private readonly Toggle _screenEdgeMouseScroll;

        private readonly Button _back;

        private readonly IHideNotify _hideNotify;

        public GameView(VisualElement root, SettingsView parent, IHideNotify hideNotify)
        {
            _root = root;
            _parent = parent;
            _hideNotify = hideNotify;

            var template = Resources.Load<VisualTreeAsset>("UI/Markup/Menus/Settings/Game");
            _tree = template.CloneTree();

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
            GameSettings.Instance.ShowHelpPanelAtStart = !_showHelpPanelAtStart.value;
        }

        private void OnCameraSensitivityChanged(ChangeEvent<float> _)
        {
            GameSettings.Instance.CameraSensitivity = _cameraSensitivity.value;
        }

        private void OnScreenEdgeToggle(ChangeEvent<bool> _)
        {
            GameSettings.Instance.ScreenEdgeMouseScroll = _screenEdgeMouseScroll.value;
        }

        private void UpdateParameters()
        {
            _showHelpPanelAtStart.value = !GameSettings.Instance.ShowHelpPanelAtStart;
            _cameraSensitivity.value = GameSettings.Instance.CameraSensitivity;
            _screenEdgeMouseScroll.value = GameSettings.Instance.ScreenEdgeMouseScroll;
        }

        private void Back()
        {
            HideSelf();
        }
    }
}
