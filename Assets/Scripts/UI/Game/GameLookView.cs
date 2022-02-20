using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(ResourcesView))]
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(UnitTypesView))]
    public class GameLookView : MonoBehaviour
    {
        private VisualElement _tree;

        private ResourcesView _resourcesView;
        private InfoPanelView _infoPanelView;
        private UnitTypesView _unitTypesView;
        private BuildVersionView _buildVersionView;

        private VisualElement _resources;
        private VisualElement _infoPanel;
        private VisualElement _unitTypes;
        private VisualElement _buildVersion;

        private void Awake()
        {
            _resourcesView = GetComponent<ResourcesView>();
            _infoPanelView = GetComponent<InfoPanelView>();
            _unitTypesView = GetComponent<UnitTypesView>();
            _buildVersionView = GetComponent<BuildVersionView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            _resources = _tree.Q<VisualElement>("resources");
            _infoPanel = _tree.Q<VisualElement>("info-panel");
            _unitTypes = _tree.Q<VisualElement>("unit-types");
            _buildVersion = _tree.Q<VisualElement>("build-version");
        }

        private void Start()
        {
            _resources.Add(_resourcesView.Tree);
            _infoPanel.Add(_infoPanelView.Tree);
            _unitTypes.Add(_unitTypesView.Tree);
            _buildVersion.Add(_buildVersionView.Tree);
        }
    }
}
