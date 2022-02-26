using System;
using System.Collections.Generic;
using Environment.TimeCycle.Seasons;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Environment.Weather
{
    public class WeatherCycle : MonoBehaviour
    {
        [SerializeField] private float _weatherEventChancePerDay = 0.4f;
        
        [Title("Weather Event Necessary Conditions")]
        [SerializeField] private List<WeatherEventNecessaryConditions> _weatherEventNecessaryConditions;

        private Season _season;
        
        private SeasonCycle _seasonCycle;

        private TimeWeatherView _timeWeatherView;
        private Temperature _temperature;

        [Inject]
        public void Construct(SeasonCycle seasonCycle, Temperature temperature, TimeWeatherView timeWeatherView)
        {
            _seasonCycle = seasonCycle;
            _temperature = temperature;
            _timeWeatherView = timeWeatherView;

            _season = Season.Spring;
        }

        private void Start()
        {
            UpdateWeather();
        }

        private void OnEnable()
        {
            _seasonCycle.SeasonChange += OnSeasonChange;
            _seasonCycle.SeasonDayChange += UpdateWeather;
        }

        private void OnDisable()
        {
            _seasonCycle.SeasonChange -= OnSeasonChange;
            _seasonCycle.SeasonDayChange -= UpdateWeather;
        }

        private void OnSeasonChange(Season season)
        {
            _season = season;
        }

        private void UpdateWeather()
        {
            var dayTemperature = _temperature.DayTemperature;

            var possibleWeather = CalculatePossibleWeather(dayTemperature);

            var weather = possibleWeather[Random.Range(0, possibleWeather.Count)];
            
            UpdateView(weather);
        }

        private List<Weather> CalculatePossibleWeather(int dayTemperature)
        {
            var possibleWeather = new List<Weather>();

            foreach (var necessaryConditions in _weatherEventNecessaryConditions)
            {
                if (necessaryConditions.SuitableWith(_season, dayTemperature))
                {
                    possibleWeather.Add(necessaryConditions.Weather);
                }
            }
            
            return possibleWeather;
        }

        [Serializable]
        private class WeatherEventNecessaryConditions
        {
            [SerializeField] private Weather _weather;
            [EnumToggleButtons]
            [SerializeField] private Season _seasons;
            
            public Weather Weather => _weather;

            public bool SuitableWith(Season season, int dayTemperature)
            {
                return _seasons.HasFlag(season);
            }
        }

        private void UpdateView(Weather weather)
        {
            _timeWeatherView.UpdateWeather(weather);
        }
    }
}
