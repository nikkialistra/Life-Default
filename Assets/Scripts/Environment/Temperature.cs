using System;
using Environment.TimeCycle.Seasons;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Environment
{
    public class Temperature : MonoBehaviour
    {
        [SerializeField] private TemperatureRange _springRange;
        [SerializeField] private TemperatureRange _summerRange;
        [SerializeField] private TemperatureRange _autumnRange;
        [SerializeField] private TemperatureRange _winterRange;

        private TemperatureRange _activeRange;
        
        private int _temperature;

        private SeasonCycle _seasonCycle;
        private TimeWeatherView _timeWeatherView;

        [Inject]
        public void Construct(SeasonCycle seasonCycle, TimeWeatherView timeWeatherView)
        {
            _seasonCycle = seasonCycle;
            _timeWeatherView = timeWeatherView;

            _activeRange = _springRange;
        }

        private void Start()
        {
            OnSeasonDayChange();
        }

        private void OnEnable()
        {
            _seasonCycle.SeasonDayChange += OnSeasonDayChange;
            _seasonCycle.SeasonChange += OnSeasonChange;
        }

        private void OnDisable()
        {
            _seasonCycle.SeasonDayChange -= OnSeasonDayChange;
            _seasonCycle.SeasonChange -= OnSeasonChange;
        }

        private void OnSeasonDayChange()
        {
            _temperature = _activeRange.GetRandomTemperature();

            UpdateView();
        }

        private void OnSeasonChange(Season season)
        {
            _activeRange = season switch {
                Season.Spring => _springRange,
                Season.Summer => _summerRange,
                Season.Autumn => _autumnRange,
                Season.Winter => _winterRange,
                _ => throw new ArgumentOutOfRangeException(nameof(season), season, null)
            };
        }

        private void UpdateView()
        {
            _timeWeatherView.ChangeTemperature(_temperature);
        }

        [Serializable]
        private struct TemperatureRange
        {
            [SerializeField] public int _lowThreshold;
            [SerializeField] public int _highThreshold;

            public int GetRandomTemperature()
            {
                return Random.Range(_lowThreshold, _highThreshold);
            }
        }
    }
}
