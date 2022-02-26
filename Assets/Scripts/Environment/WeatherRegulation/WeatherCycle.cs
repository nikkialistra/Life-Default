using System;
using System.Collections.Generic;
using Environment.TemperatureRegulation;
using Environment.TimeCycle.Days;
using Environment.TimeCycle.Seasons;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Environment.WeatherRegulation
{
    public class WeatherCycle : MonoBehaviour
    {
        [SerializeField] private float _weatherEventChancePerDay = 0.4f;
        [SerializeField] private DayRange _weatherBeginTimeRange;

        [Title("Weather Event Necessary Conditions")]
        [SerializeField] private List<WeatherEventNecessaryConditions> _weatherEventNecessaryConditions;

        private Weather _currentWeather;

        private WeatherEventNecessaryConditions _futureWeatherWithConditions;
        private int _weatherBeginTime;

        private bool _weatherChangePending;
        
        private Season _season;

        private SeasonCycle _seasonCycle;

        private TimeWeatherView _timeWeatherView;
        private Temperature _temperature;
        private DayCycle _dayCycle;

        [Inject]
        public void Construct(SeasonCycle seasonCycle, DayCycle dayCycle, Temperature temperature, TimeWeatherView timeWeatherView)
        {
            _seasonCycle = seasonCycle;
            _dayCycle = dayCycle;
            _temperature = temperature;
            _timeWeatherView = timeWeatherView;

            _season = Season.Spring;
        }

        private void Start()
        {
            FindNewFutureWeather();
        }

        private void OnEnable()
        {
            _dayCycle.HourChange += OnHourChange;
            
            _seasonCycle.SeasonChange += OnSeasonChange;
            _seasonCycle.SeasonDayChange += FindNewFutureWeather;
        }

        private void OnDisable()
        {
            _dayCycle.HourChange -= OnHourChange;
            
            _seasonCycle.SeasonChange -= OnSeasonChange;
            _seasonCycle.SeasonDayChange -= FindNewFutureWeather;
        }

        private void OnHourChange(int hour)
        {
            if (!_weatherChangePending)
            {
                return;
            }

            if (hour >= _weatherBeginTime && _futureWeatherWithConditions.SuitableWith(_temperature.CurrentBaseTemperature))
            {
                _currentWeather = _futureWeatherWithConditions.Weather;
                _weatherChangePending = false;
                UpdateView();
            }
        }

        private void OnSeasonChange(Season season)
        {
            _season = season;
        }

        private void FindNewFutureWeather()
        {
            if (WeatherEventOccured)
            {
                _futureWeatherWithConditions = _weatherEventNecessaryConditions[7];
            }
            else
            {
                _futureWeatherWithConditions = _weatherEventNecessaryConditions[7];
            }

            _weatherBeginTime = _weatherBeginTimeRange.GetRandomHour();
            _weatherChangePending = true;
        }

        private bool WeatherEventOccured => Random.Range(0, 1f) <= _weatherEventChancePerDay;

        private void GenerateWeather()
        {
            var dayTemperature = _temperature.DayTemperature;

            var possibleWeather = CalculatePossibleWeather(dayTemperature);

            _futureWeatherWithConditions = possibleWeather[Random.Range(0, possibleWeather.Count)];
        }

        private List<WeatherEventNecessaryConditions> CalculatePossibleWeather(int dayTemperature)
        {
            var possibleWeather = new List<WeatherEventNecessaryConditions>();

            foreach (var necessaryConditions in _weatherEventNecessaryConditions)
            {
                if (necessaryConditions.SuitableWith(_season, dayTemperature))
                {
                    possibleWeather.Add(necessaryConditions);
                }
            }
            
            return possibleWeather;
        }

        [Serializable]
        private class WeatherEventNecessaryConditions
        {
            [GUIColor(.4f, .4f, 1f)]
            [SerializeField] private Weather _weather;
            [Space]
            [EnumToggleButtons]
            [SerializeField] private Season _seasons;
            [SerializeField] private TemperatureRange _temperatureRange;

            public Weather Weather => _weather;

            public bool SuitableWith(Season season, int dayTemperature)
            {
                return _seasons.HasFlag(season) && _temperatureRange.Contains(dayTemperature);
            }
            
            public bool SuitableWith(int dayTemperature)
            {
                return _temperatureRange.Contains(dayTemperature);
            }
        }

        private void UpdateView()
        {
            _timeWeatherView.UpdateWeather(_currentWeather);
        }
    }
}
