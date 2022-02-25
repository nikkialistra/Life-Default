using System;
using Environment.TimeCycle.Days;
using Environment.TimeCycle.Seasons;
using Environment.TimeCycle.Ticking;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Environment
{
    public class Temperature : MonoBehaviour, ITicking
    {
        [Title("Shifting")]
        [SerializeField] private int _nightToDayValueDifference = -10;
        [SerializeField] private int _ticksToShift = 25;

        [Title("Temperature Ranges")]
        [SerializeField] private TemperatureRange _springRange;
        [SerializeField] private TemperatureRange _summerRange;
        [SerializeField] private TemperatureRange _autumnRange;
        [SerializeField] private TemperatureRange _winterRange;

        private TemperatureRange _activeRange;

        private float _currentTemperature;
        
        private int _dayTemperature;

        private float _valueShiftPerTick;
        private bool _shifting;
        private int _shiftTickCount;

        private SeasonCycle _seasonCycle;
        private DayCycle _dayCycle;

        private TimeWeatherView _timeWeatherView;

        [Inject]
        public void Construct(SeasonCycle seasonCycle, DayCycle dayCycle, TickingRegulator tickingRegulator, TimeWeatherView timeWeatherView)
        {
            _seasonCycle = seasonCycle;
            _dayCycle = dayCycle;

            _timeWeatherView = timeWeatherView;
            
            _activeRange = _springRange;
            
            tickingRegulator.AddToTickables(this);
        }

        private void Start()
        {
            OnSeasonDayChange();
            _currentTemperature = _dayTemperature;
        }

        private void OnEnable()
        {
            _seasonCycle.SeasonDayChange += OnSeasonDayChange;
            _seasonCycle.SeasonChange += OnSeasonChange;

            _dayCycle.DayBegin += OnDayBegin;
            _dayCycle.NightBegin += OnNightBegin;
        }

        private void OnDisable()
        {
            _seasonCycle.SeasonDayChange -= OnSeasonDayChange;
            _seasonCycle.SeasonChange -= OnSeasonChange;
            
            _dayCycle.DayBegin -= OnDayBegin;
            _dayCycle.NightBegin -= OnNightBegin;
        }

        public void Tick()
        {
            if (!_shifting)
            {
                return;
            }

            Shift();
            CheckForShiftFinish();
            
            UpdateView();
        }

        private void Shift()
        {
            _currentTemperature += _valueShiftPerTick;
            _shiftTickCount++;
        }

        private void CheckForShiftFinish()
        {
            if (_shiftTickCount == _ticksToShift)
            {
                _shifting = false;
                _shiftTickCount = 0;
            }
        }

        private void OnSeasonDayChange()
        {
            _dayTemperature = _activeRange.GetRandomTemperature();
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

        private void OnDayBegin()
        {
            _valueShiftPerTick = (_dayTemperature - _currentTemperature) / _ticksToShift;
            _shifting = true;
        }

        private void OnNightBegin()
        {
            _valueShiftPerTick = (NightTemperature - _currentTemperature) / _ticksToShift;
            _shifting = true;
        }

        private int NightTemperature => _dayTemperature + _nightToDayValueDifference;

        private void UpdateView()
        {
            _timeWeatherView.ChangeTemperature(Mathf.RoundToInt(_currentTemperature));
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
