using System;
using Environment.TimeCycle.Days;
using Environment.TimeCycle.Seasons;
using Environment.TimeCycle.Ticking;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Environment.TemperatureRegulation
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

        private float _currentBaseTemperature;

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

        public int DayTemperature { get; private set; }
        public int CurrentBaseTemperature => Mathf.RoundToInt(_currentBaseTemperature);

        private void Start()
        {
            OnSeasonDayChange();
            _currentBaseTemperature = DayTemperature;
            UpdateView();
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
            _currentBaseTemperature += _valueShiftPerTick;
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
            DayTemperature = _activeRange.GetRandomTemperature();
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
            _valueShiftPerTick = (DayTemperature - _currentBaseTemperature) / _ticksToShift;
            _shifting = true;
        }

        private void OnNightBegin()
        {
            _valueShiftPerTick = (NightTemperature - _currentBaseTemperature) / _ticksToShift;
            _shifting = true;
        }

        private int NightTemperature => DayTemperature + _nightToDayValueDifference;

        private void UpdateView()
        {
            _timeWeatherView.UpdateTemperature(Mathf.RoundToInt(_currentBaseTemperature));
        }
    }
}
