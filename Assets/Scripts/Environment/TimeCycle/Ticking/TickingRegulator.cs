using System;
using System.Collections.Generic;
using Environment.TimeCycle.TimeRegulation;
using UnityEngine;
using Zenject;

namespace Environment.TimeCycle.Ticking
{
    public class TickingRegulator : MonoBehaviour
    {
        private readonly List<ITicking> _tickables = new();

        private float _seconds;
        
        private int _tickSpeed = 1;
        
        private bool _paused;
        private TimeSpeed _timeSpeed;

        private TimeToggling _timeToggling;

        [Inject]
        public void Construct(TimeToggling timeToggling)
        {
            _timeToggling = timeToggling;
        }

        private void OnEnable()
        {
            _timeToggling.PauseChange += OnPauseChange;
            _timeToggling.TimeSpeedChange += OnTimeSpeedChange;
        }

        private void OnDisable()
        {
            _timeToggling.PauseChange -= OnPauseChange;
            _timeToggling.TimeSpeedChange -= OnTimeSpeedChange;
        }

        public void AddToTickables(ITicking tickable)
        {
            _tickables.Add(tickable);
        }

        private void Update()
        {
            _seconds += Time.deltaTime * _tickSpeed;

            if (_seconds > 1f)
            {
                _seconds -= 1f;
                Tick();
            }
        }

        private void OnPauseChange(bool paused)
        {
            _paused = paused;
            CalculateTickSpeed();
        }

        private void OnTimeSpeedChange(TimeSpeed timeSpeed)
        {
            _timeSpeed = timeSpeed;
            CalculateTickSpeed();
        }

        private void CalculateTickSpeed()
        {
            if (_paused)
            {
                _tickSpeed = 0;
            }
            else
            {
                _tickSpeed = _timeSpeed switch
                {
                    TimeSpeed.X1 => 1,
                    TimeSpeed.X2 => 2,
                    TimeSpeed.X3 => 3,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private void Tick()
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }
    }
}
