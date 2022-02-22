using Game;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(TimeTogglingView))]
    [RequireComponent(typeof(TimeWeatherView))]
    [RequireComponent(typeof(MenuPanelView))]
    [RequireComponent(typeof(ResourcesView))]
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(UnitTypesView))]
    public class GameLookView : MonoBehaviour
    {
        [Required]
        [SerializeField] private TimeToggling _timeToggling;

        private VisualElement _tree;
        private VisualElement _gameLook;

        private BuildVersionView _buildVersionView;
        private TimeTogglingView _timeTogglingView;
        private TimeWeatherView _timeWeatherView;
        private MenuPanelView _menuPanelView;
        private ResourcesView _resourcesView;
        private InfoPanelView _infoPanelView;
        private UnitTypesView _unitTypesView;

        private VisualElement _buildVersionElement;
        private VisualElement _timeTogglingElement;
        private VisualElement _timeWeatherElement;
        private VisualElement _menuPanelElement;
        private VisualElement _resourcesElement;
        private VisualElement _infoPanelElement;
        private VisualElement _unitTypesElement;

        private void Awake()
        {
            _buildVersionView = GetComponent<BuildVersionView>();
            _timeTogglingView = GetComponent<TimeTogglingView>();
            _timeWeatherView = GetComponent<TimeWeatherView>();
            _menuPanelView = GetComponent<MenuPanelView>();
            _resourcesView = GetComponent<ResourcesView>();
            _infoPanelView = GetComponent<InfoPanelView>();
            _unitTypesView = GetComponent<UnitTypesView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            _gameLook = _tree.Q<VisualElement>("game-look");

            _buildVersionElement = _tree.Q<VisualElement>("build-version");
            _timeTogglingElement = _tree.Q<VisualElement>("time-toggling");
            _timeWeatherElement = _tree.Q<VisualElement>("time-weather");
            _menuPanelElement = _tree.Q<VisualElement>("menu-panel");
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
            _timeWeatherElement.Add(_timeWeatherView.Tree);
            _menuPanelElement.Add(_menuPanelView.Tree);
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
