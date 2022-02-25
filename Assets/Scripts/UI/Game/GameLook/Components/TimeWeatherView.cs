using Environment;
using Environment.Indicators;
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
        private Label _time;
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

            _time = Tree.Q<Label>("time");

            _localTemperature = Tree.Q<Label>("local-temperature");
            _weather = Tree.Q<Label>("weather");
        }
        
        public VisualElement Tree { get; private set; }

        public void ChangeYear(int year)
        {
            _year.text = year.ToString();
        }

        public void ChangeSeason(Season season)
        {
            _season = season;
            UpdateSeasonDay();
        }

        public void ChangeDay(int day)
        {
            _day = day;
            UpdateSeasonDay();
        }

        public void ChangeTime(int time)
        {
            _time.text = time + " h";
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
