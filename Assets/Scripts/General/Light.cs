using System;
using General.TimeCycle.Days;
using General.TimeCycle.Seasons;
using General.TimeCycle.Ticking;
using General.WeatherRegulation;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace General
{
    public class Light : MonoBehaviour, ITicking
    {
        [SerializeField] private int _ticksToShift = 10;
        
        [Title("Season Light")]
        [SerializeField] private int _springLight;
        [SerializeField] private int _summerLight;
        [SerializeField] private int _autumnLight;
        [SerializeField] private int _winterLight;

        private float _currentBaseLight;
        private float _currentInfluencedLight;
        
        private int _seasonLight;

        private float _valueShiftPerTick;
        private bool _shifting;
        private int _shiftTickCount;
        
        private SeasonCycle _seasonCycle;
        private DayCycle _dayCycle;
        private WeatherEnvironmentInfluence _weatherEnvironmentInfluence;

        private TimeWeatherView _timeWeatherView;

        [Inject]
        public void Construct(SeasonCycle seasonCycle, DayCycle dayCycle,
            WeatherEnvironmentInfluence weatherEnvironmentInfluence, TickingRegulator tickingRegulator,
            TimeWeatherView timeWeatherView)
        {
            _seasonCycle = seasonCycle;
            _dayCycle = dayCycle;
            _weatherEnvironmentInfluence = weatherEnvironmentInfluence;

            _timeWeatherView = timeWeatherView;

            _seasonLight = _springLight;

            tickingRegulator.AddToTickables(this);
        }

        private void OnEnable()
        {
            _seasonCycle.SeasonChange += OnSeasonChange;

            _dayCycle.DayBegin += OnDayBegin;
            _dayCycle.NightBegin += OnNightBegin;
        }

        private void OnDisable()
        {
            _seasonCycle.SeasonChange -= OnSeasonChange;

            _dayCycle.DayBegin -= OnDayBegin;
            _dayCycle.NightBegin -= OnNightBegin;
        }

        private void Start()
        {
            _currentBaseLight = _seasonLight;
            UpdateView();
        }

        public void Tick()
        {
            if (_shifting)
            {
                Shift();
                CheckForShiftFinish();
            }
            
            CalculateInfluencedLight();

            UpdateView();
        }

        private void Shift()
        {
            _currentBaseLight += _valueShiftPerTick;
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

        private void CalculateInfluencedLight()
        {
            _currentInfluencedLight = Mathf.Clamp(_currentBaseLight + _weatherEnvironmentInfluence.LightSumModifier, 0, 100);
        }

        private void OnSeasonChange(Season season)
        {
            _seasonLight = season switch {
                Season.Spring => _springLight,
                Season.Summer => _summerLight,
                Season.Autumn => _autumnLight,
                Season.Winter => _winterLight,
                _ => throw new ArgumentOutOfRangeException(nameof(season), season, null)
            };
        }

        private void OnDayBegin()
        {
            _valueShiftPerTick = (_seasonLight - _currentBaseLight) / _ticksToShift;
            _shifting = true;
        }

        private void OnNightBegin()
        {
            _valueShiftPerTick = (0 - _currentBaseLight) / _ticksToShift;
            _shifting = true;
        }

        private void UpdateView()
        {
            _timeWeatherView.UpdateLight(Mathf.RoundToInt(_currentInfluencedLight));
        }
    }
}
