﻿using Medium.TimeCycle.TimeRegulation;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UI.Game.GameLook.Components.Info;
using UI.Game.GameLook.Components.Stock;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(NotificationsView))]
    [RequireComponent(typeof(BuildVersionView))]
    [RequireComponent(typeof(LogMessageCountsView))]
    [RequireComponent(typeof(ColonistIconsView))]
    [RequireComponent(typeof(TimeWeatherView))]
    [RequireComponent(typeof(TimeTogglingView))]
    [RequireComponent(typeof(TileInfoView))]
    public class GameLookView : MonoBehaviour
    {
        [Required]
        [SerializeField] private StockView _stockView;
        [Required]
        [SerializeField] private InfoPanelView _infoPanelView;

        private VisualElement _tree;
        private VisualElement _gameLook;

        private BuildVersionView _buildVersionView;
        private NotificationsView _notificationsView;
        private LogMessageCountsView _logMessageCountsView;
        private ColonistIconsView _colonistIconsView;
        private TimeTogglingView _timeTogglingView;
        private TimeWeatherView _timeWeatherView;
        private TileInfoView _tileInfoView;

        private VisualElement _stockElement;
        private VisualElement _notificationsElement;
        private VisualElement _colonistIconsElement;
        private VisualElement _buildVersionElement;
        private VisualElement _logMessageCountsElement;
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
            _notificationsView = GetComponent<NotificationsView>();
            _colonistIconsView = GetComponent<ColonistIconsView>();
            _buildVersionView = GetComponent<BuildVersionView>();
            _logMessageCountsView = GetComponent<LogMessageCountsView>();
            _timeWeatherView = GetComponent<TimeWeatherView>();
            _timeTogglingView = GetComponent<TimeTogglingView>();
            _tileInfoView = GetComponent<TileInfoView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            _gameLook = _tree.Q<VisualElement>("game-look");

            _stockElement = _tree.Q<VisualElement>("stock");
            _notificationsElement = _tree.Q<VisualElement>("notifications");
            _colonistIconsElement = _tree.Q<VisualElement>("colonist-icons");
            _buildVersionElement = _tree.Q<VisualElement>("build-version");
            _logMessageCountsElement = _tree.Q<VisualElement>("log-message-counts");
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
            _notificationsElement.Add(_notificationsView.Tree);
            _colonistIconsElement.Add(_colonistIconsView.Tree);
            _buildVersionElement.Add(_buildVersionView.Tree);
            _logMessageCountsElement.Add(_logMessageCountsView.Tree);
            _timeWeatherElement.Add(_timeWeatherView.Tree);
            _timeTogglingElement.Add(_timeTogglingView.Tree);
            _tileInfoElement.Add(_tileInfoView.Tree);
            _infoPanelElement.Add(_infoPanelView.Tree);
        }

        public void ToggleTimeSpeedMultipliedIndicator(bool timeSpeedMultiplied)
        {
            if (timeSpeedMultiplied)
                _gameLook.AddToClassList("time-speed-multiplied");
            else
                _gameLook.RemoveFromClassList("time-speed-multiplied");
        }

        private void OnPauseChange(bool paused)
        {
            if (paused)
                _gameLook.AddToClassList("paused");
            else
                _gameLook.RemoveFromClassList("paused");
        }
    }
}
