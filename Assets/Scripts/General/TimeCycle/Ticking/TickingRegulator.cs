using System;
using System.Collections.Generic;
using General.TimeCycle.TimeRegulation;
using UnityEngine;
using Zenject;

namespace General.TimeCycle.Ticking
{
    public class TickingRegulator : MonoBehaviour
    {
        private readonly List<ITickable> _tickables = new();
        private readonly List<ITickablePerHour> _tickablesEveryHour = new();

        private float _seconds;
        
        private int _tickSpeed = 1;
        
        private bool _paused;
        private TimeSpeed _timeSpeed;

        private TimeToggling _timeToggling;
        
        private int _tickCount;

        [Inject]
        public void Construct(TimeToggling timeToggling)
        {
            _timeToggling = timeToggling;
        }

        private void Start()
        {
            Tick();
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

        public void AddToTickables(ITickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void AddToTickablesPerHour(ITickablePerHour tickablePerHour)
        {
            _tickablesEveryHour.Add(tickablePerHour);
        }

        public void RemoveFromTickablesPerHour(ITickablePerHour tickablePerHour)
        {
            _tickablesEveryHour.Remove(tickablePerHour);
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

            TickEveryHour();
            _tickCount++;
        }

        private void TickEveryHour()
        {
            if (_tickCount % TickCounts.Hour == 0)
            {
                foreach (var tickablePerHour in _tickablesEveryHour)
                {
                    tickablePerHour.TickPerHour();
                }
            }
        }
    }
}
