using General.TimeCycle.Seasons;
using General.WeatherRegulation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TimeWeatherView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;
        
        [Title("Time Status")]
        [Required]
        [SerializeField] private Sprite _iconDay;
        [Required]
        [SerializeField] private Sprite _iconNight;
        
        [Title("Weather Icons")]
        [Required]
        [SerializeField] private Sprite _iconClear;

        private Label _day;
        private Label _seasonYear;

        private VisualElement _timeStatus;
        private Label _temperature;

        private VisualElement _weatherIcon;
        private Label _light;
        private Label _weather;

        private Label _time;

        private void Awake()
        {
            Tree = _asset.CloneTree();

            _day = Tree.Q<Label>("day");
            _seasonYear = Tree.Q<Label>("season-year");
            
            _timeStatus = Tree.Q<VisualElement>("time-status");
            _temperature = Tree.Q<Label>("temperature");

            _weatherIcon = Tree.Q<VisualElement>("weather-icon");
            _light = Tree.Q<Label>("light");
            _weather = Tree.Q<Label>("weather");

            _time = Tree.Q<Label>("time");
        }
        
        public VisualElement Tree { get; private set; }

        public void UpdateDate(int day, Season season, int year)
        {
            _day.text = $"Day {day} of";
            _seasonYear.text = $"{season}, {year}";
        }

        public void UpdateTemperature(int temperature)
        {
            _temperature.text = $"{temperature} °C";
        }

        public void UpdateWeather(Weather weather)
        {
            _weather.text = weather.GetString();

            _weatherIcon.style.backgroundImage = weather switch
            {
                Weather.Clear => new StyleBackground(_iconClear),
                _ => new StyleBackground(_iconClear)
            };
        }

        public void UpdateLight(int light)
        {
            _light.text = $"{light}% Lit,";
        }

        public void UpdateTime(int hours, int minutes)
        {
            _time.text = $"{hours}:{minutes:D2}";

            _timeStatus.style.backgroundImage =
                hours <= 5 ? new StyleBackground(_iconNight) : new StyleBackground(_iconDay);
        }
    }
}
