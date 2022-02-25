using Environment.Indicators;
using Environment.TimeCycle.Seasons;
using Environment.Weather;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TimeWeatherView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/TimeWeather";
        
        private Label _year;
        private Label _seasonDay;
        private Label _hours;
        private Label _localTemperature;
        private Label _weather;
        
        private Season _season;
        private int _day;
        
        private int _temperature;
        private Openness _openness;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _year = Tree.Q<Label>("year");
            _seasonDay = Tree.Q<Label>("season-day");

            _hours = Tree.Q<Label>("hours");

            _localTemperature = Tree.Q<Label>("local-temperature");
            _weather = Tree.Q<Label>("weather");
        }
        
        public VisualElement Tree { get; private set; }

        public void ChangeSeasonInfo(Season season, int day, int year)
        {
            _season = season;
            _day = day;
            UpdateSeasonDay();
            
            _year.text = year.ToString();
        }

        public void ChangeHours(int hours)
        {
            _hours.text = hours + " h";
        }

        public void ChangeTemperature(int temperature)
        {
            _temperature = temperature;
            UpdateLocalTemperature();
        }

        public void ChangeOpenness(Openness openness)
        {
            _openness = openness;
            UpdateLocalTemperature();
        }

        public void ChangeWeather(Weather weather)
        {
            _weather.text = weather.GetString();
        }

        private void UpdateSeasonDay()
        {
            _seasonDay.text = _season + ", day " + _day;
        }

        private void UpdateLocalTemperature()
        {
            _localTemperature.text = _temperature + " °C, " + _openness;
        }
    }
}
