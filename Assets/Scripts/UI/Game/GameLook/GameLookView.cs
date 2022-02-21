using UI.Game.GameLook.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(ResourcesView))]
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(UnitTypesView))]
    [RequireComponent(typeof(TimeTogglingView))]
    public class GameLookView : MonoBehaviour
    {
        private VisualElement _tree;

        private BuildVersionView _buildVersionView;
        private TimeTogglingView _timeTogglingView;
        private ResourcesView _resourcesView;
        private InfoPanelView _infoPanelView;
        private UnitTypesView _unitTypesView;

        private VisualElement _buildVersion;
        private VisualElement _timeToggling;
        private VisualElement _resources;
        private VisualElement _infoPanel;
        private VisualElement _unitTypes;

        private void Awake()
        {
            _buildVersionView = GetComponent<BuildVersionView>();
            _timeTogglingView = GetComponent<TimeTogglingView>();
            _resourcesView = GetComponent<ResourcesView>();
            _infoPanelView = GetComponent<InfoPanelView>();
            _unitTypesView = GetComponent<UnitTypesView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            _buildVersion = _tree.Q<VisualElement>("build-version");
            _timeToggling = _tree.Q<VisualElement>("time-toggling");
            _resources = _tree.Q<VisualElement>("resources");
            _infoPanel = _tree.Q<VisualElement>("info-panel");
            _unitTypes = _tree.Q<VisualElement>("unit-types");
        }

        private void Start()
        {
            _buildVersion.Add(_buildVersionView.Tree);
            _timeToggling.Add(_timeTogglingView.Tree);
            _resources.Add(_resourcesView.Tree);
            _infoPanel.Add(_infoPanelView.Tree);
            _unitTypes.Add(_unitTypesView.Tree);
        }
    }
}
