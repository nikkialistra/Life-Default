using System;
using Environment.TimeCycle.Seasons;
using Environment.TimeCycle.Ticking;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Environment.TimeCycle.Days
{
    public class DayCycle : MonoBehaviour, ITicking
    {
        [Range(0, 23)]
        [SerializeField] private int _hours = 23;
        
        private int _ticks;
        private int _minutes;
        
        private SeasonCycle _seasonCycle;
        private TimeWeatherView _timeWeatherView;

        [Inject]
        public void Construct(SeasonCycle seasonCycle, TickingRegulator tickingRegulator, TimeWeatherView timeWeatherView)
        {
            _seasonCycle = seasonCycle;
            _timeWeatherView = timeWeatherView;
            
            tickingRegulator.AddToTickables(this);
        }

        public event Action<int> HourChange;
        public event Action DayBegin;
        public event Action NightBegin;

        private void Start()
        {
            UpdateView();
        }

        public void Tick()
        {
            _ticks++;

            UpdateMinutes();

            if (_ticks == 25)
            {
                _ticks = 0;
                UpdateHours();
            }
            
            UpdateView();
        }

        private void UpdateMinutes()
        {
            _minutes = Mathf.RoundToInt((float)_ticks / 25 * 60);

            _minutes -= _minutes % 10;
            
            if (_minutes == 60)
            {
                _minutes = 0;
            }
        }

        private void UpdateHours()
        {
            _hours++;

            if (_hours == 6)
            {
                DayBegin?.Invoke();
            }
            
            if (_hours == 24)
            {
                _hours = 0;
                _seasonCycle.NextDay();
                NightBegin?.Invoke();
            }
            
            HourChange?.Invoke(_hours);
        }

        private void UpdateView()
        {
            _timeWeatherView.UpdateTime(_hours, _minutes);
        }
    }
}
