using Environment.TimeCycle.Seasons;
using Environment.Weather;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TimeWeatherView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/TimeWeather";
        
        private Label _yearAndLight;
        private Label _seasonDay;
        private Label _hours;
        private Label _temperature;
        private Label _weather;
        
        private Season _season;
        private int _day;

        private int _year;
        private int _light;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _yearAndLight = Tree.Q<Label>("year-and-light");
            _seasonDay = Tree.Q<Label>("season-day");

            _hours = Tree.Q<Label>("hours");

            _temperature = Tree.Q<Label>("local-temperature");
            _weather = Tree.Q<Label>("weather");
        }
        
        public VisualElement Tree { get; private set; }

        public void ChangeSeasonInfo(Season season, int day, int year)
        {
            _season = season;
            _day = day;
            UpdateSeasonDay();
            
            _year = year;
            UpdateYearAndLight();
        }

        public void UpdateHours(int hours)
        {
            _hours.text = $"{hours} h";
        }

        public void UpdateTemperature(int temperature)
        {
            _temperature.text = $"{temperature} °C Outside";
        }

        public void UpdateLight(int light)
        {
            _light = light;
            UpdateYearAndLight();
        }

        public void UpdateWeather(Weather weather)
        {
            _weather.text = weather.GetString();
        }

        private void UpdateSeasonDay()
        {
            _seasonDay.text = $"{_season}, day {_day}";
        }

        private void UpdateYearAndLight()
        {
            _yearAndLight.text = $"{_year}, {_light}% Lit";
        }
    }
}
