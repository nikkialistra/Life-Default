using System;
using System.Collections.Generic;
using Common;
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

        [ValidateInput("EveryWeatherHasConditions", "Not every weather has conditions")]
        [Title("Weather Necessary Conditions")]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        [SerializeField] private WeatherNecessaryConditionsDictionary _weatherNecessaryConditions;

        private Weather _currentWeather;
        
        private Weather _futureWeather;
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
        
        private bool EveryWeatherHasConditions(WeatherNecessaryConditionsDictionary conditions, ref string errorMessage)
        {
            foreach (var weatherValue in (Weather[])Enum.GetValues(typeof(Weather)))
            {
                if (!conditions.ContainsKey(weatherValue))
                {
                    errorMessage = $"{weatherValue} don't have conditions";
                    return false;
                }
            }

            return true;
        }

        private void OnHourChange(int hour)
        {
            if (!_weatherChangePending)
            {
                return;
            }

            if (hour >= _weatherBeginTime && _weatherNecessaryConditions[_futureWeather].SuitableWith(_temperature.CurrentBaseTemperature))
            {
                _currentWeather = _futureWeather;
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
                GenerateWeather();
            }
            else
            {
                _futureWeather = Weather.Clear;
            }

            _weatherBeginTime = _weatherBeginTimeRange.GetRandomHour();
            _weatherChangePending = true;
        }

        private bool WeatherEventOccured => Random.Range(0, 1f) <= _weatherEventChancePerDay;

        private void GenerateWeather()
        {
            var dayTemperature = _temperature.DayTemperature;

            var possibleWeather = CalculatePossibleWeather(dayTemperature);

            _futureWeather = possibleWeather[Random.Range(0, possibleWeather.Count)];
        }

        private List<Weather> CalculatePossibleWeather(int dayTemperature)
        {
            var possibleWeather = new List<Weather>();

            foreach (var (weather, necessaryConditions) in _weatherNecessaryConditions)
            {
                if (necessaryConditions.SuitableWith(_season, dayTemperature))
                {
                    possibleWeather.Add(weather);
                }
            }
            
            return possibleWeather;
        }

        private void UpdateView()
        {
            _timeWeatherView.UpdateWeather(_currentWeather);
        }
        
        [Serializable] public class WeatherNecessaryConditionsDictionary : SerializableDictionary<Weather, WeatherNecessaryConditions> { }
    }
}
