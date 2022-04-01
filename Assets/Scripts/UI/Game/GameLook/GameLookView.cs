using General.TimeCycle.TimeRegulation;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(StockView))]
    [RequireComponent(typeof(BuildVersionView))]
    [RequireComponent(typeof(ColonistIconsView))]
    [RequireComponent(typeof(TimeWeatherView))]
    [RequireComponent(typeof(TimeTogglingView))]
    [RequireComponent(typeof(TileInfoView))]
    public class GameLookView : MonoBehaviour
    {
        [Required]
        [SerializeField] private InfoPanelView _infoPanelView;
        
        private VisualElement _tree;
        private VisualElement _gameLook;

        private StockView _stockView;
        private BuildVersionView _buildVersionView;
        private ColonistIconsView _colonistIconsView;
        private TimeTogglingView _timeTogglingView;
        private TimeWeatherView _timeWeatherView;
        private TileInfoView _tileInfoView;

        private VisualElement _stockElement;
        private VisualElement _colonistIconsElement;
        private VisualElement _buildVersionElement;
        private VisualElement _timeWeatherElement;
        private VisualElement _timeTogglingElement;
        private VisualElement _tileInfoElement;
        private VisualElement _infoPanelElement;

        private TimeToggling _timeToggling;

        [Inject]
        public void Construct(TimeToggling timeToggling)
        {
            _timeToggling = timeToggling;
        }

        private void Awake()
        {
            _stockView = GetComponent<StockView>();
            _colonistIconsView = GetComponent<ColonistIconsView>();
            _buildVersionView = GetComponent<BuildVersionView>();
            _timeWeatherView = GetComponent<TimeWeatherView>();
            _timeTogglingView = GetComponent<TimeTogglingView>();
            _tileInfoView = GetComponent<TileInfoView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            _gameLook = _tree.Q<VisualElement>("game-look");

            _stockElement = _tree.Q<VisualElement>("stock");
            _colonistIconsElement = _tree.Q<VisualElement>("colonist-icons");
            _buildVersionElement = _tree.Q<VisualElement>("build-version");
            _timeWeatherElement = _tree.Q<VisualElement>("time-weather");
            _timeTogglingElement = _tree.Q<VisualElement>("time-toggling");
            _tileInfoElement = _tree.Q<VisualElement>("tile-info");
            _infoPanelElement = _tree.Q<VisualElement>("info-panel");
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
            _stockElement.Add(_stockView.Tree);
            _colonistIconsElement.Add(_colonistIconsView.Tree);
            _buildVersionElement.Add(_buildVersionView.Tree);
            _timeWeatherElement.Add(_timeWeatherView.Tree);
            _timeTogglingElement.Add(_timeTogglingView.Tree);
            _tileInfoElement.Add(_tileInfoView.Tree);
            _infoPanelElement.Add(_infoPanelView.Tree);
        }

        public void ToggleTimeSpeedMultipliedIndicator(bool timeSpeedMultiplied)
        {
            if (timeSpeedMultiplied)
            {
                _gameLook.AddToClassList("time-speed-multiplied");
            }
            else
            {
                _gameLook.RemoveFromClassList("time-speed-multiplied");
            }
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
