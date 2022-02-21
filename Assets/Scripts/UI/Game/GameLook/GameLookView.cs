using Game;
using Sirenix.OdinInspector;
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
        [Required]
        [SerializeField] private TimeToggling _timeToggling;

        private VisualElement _tree;
        private VisualElement _gameLook;

        private BuildVersionView _buildVersionView;
        private TimeTogglingView _timeTogglingView;
        private ResourcesView _resourcesView;
        private InfoPanelView _infoPanelView;
        private UnitTypesView _unitTypesView;

        private VisualElement _buildVersionElement;
        private VisualElement _timeTogglingElement;
        private VisualElement _resourcesElement;
        private VisualElement _infoPanelElement;
        private VisualElement _unitTypesElement;

        private void Awake()
        {
            _buildVersionView = GetComponent<BuildVersionView>();
            _timeTogglingView = GetComponent<TimeTogglingView>();
            _resourcesView = GetComponent<ResourcesView>();
            _infoPanelView = GetComponent<InfoPanelView>();
            _unitTypesView = GetComponent<UnitTypesView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            _gameLook = _tree.Q<VisualElement>("game-look");

            _buildVersionElement = _tree.Q<VisualElement>("build-version");
            _timeTogglingElement = _tree.Q<VisualElement>("time-toggling");
            _resourcesElement = _tree.Q<VisualElement>("resources");
            _infoPanelElement = _tree.Q<VisualElement>("info-panel");
            _unitTypesElement = _tree.Q<VisualElement>("unit-types");
        }

        private void OnEnable()
        {
            _timeToggling.PauseChange += OnPauseChange;
        }

        private void OnDisable()
        {
            _timeToggling.PauseChange -= OnPauseChange;
        }

        private void Start()
        {
            _buildVersionElement.Add(_buildVersionView.Tree);
            _timeTogglingElement.Add(_timeTogglingView.Tree);
            _resourcesElement.Add(_resourcesView.Tree);
            _infoPanelElement.Add(_infoPanelView.Tree);
            _unitTypesElement.Add(_unitTypesView.Tree);
        }

        private void OnPauseChange(bool paused)
        {
            if (paused)
            {
                _gameLook.AddToClassList("paused");
            }
            else
            {
                _gameLook.RemoveFromClassList("paused");
            }
        }
    }
}
