using System;
using UI.Game.GameLook.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Environment.TimeCycle.TimeRegulation
{
    public class TimeToggling : MonoBehaviour
    {
        private bool _paused;
        private TimeSpeed _timeSpeed = TimeSpeed.X1;

        private TimeTogglingView _timeTogglingView;

        [Inject]
        public void Construct(TimeTogglingView timeTogglingView)
        {
            _timeTogglingView = timeTogglingView;
        }

        public event Action<bool> PauseChange;
        public event Action<TimeSpeed> TimeSpeedChange;

        public int TimeSpeedMultiplier { get; set; } = 1;

        public void Pause()
        {
            _paused = !_paused;
            ToggleTime();
            UpdateView();
            
            PauseChange?.Invoke(_paused);
        }

        public void ChangeSpeed(TimeSpeed timeSpeed)
        {
            _timeSpeed = timeSpeed;
            ToggleTime();
            UpdateView();
            
            TimeSpeedChange?.Invoke(_timeSpeed);
        }

        public void NextSpeed()
        {
            _timeSpeed = _timeSpeed switch
            {
                TimeSpeed.X1 => TimeSpeed.X2,
                TimeSpeed.X2 => TimeSpeed.X3,
                TimeSpeed.X3 => TimeSpeed.X1,
                _ => throw new ArgumentOutOfRangeException()
            };
            ToggleTime();
            UpdateView();
        }

        public void ToggleTime()
        {
            if (_paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = _timeSpeed switch
                {
                    TimeSpeed.X1 => 1f,
                    TimeSpeed.X2 => 2f,
                    TimeSpeed.X3 => 3f,
                    _ => throw new ArgumentOutOfRangeException()
                } * TimeSpeedMultiplier;
            }
        }

        private void UpdateView()
        {
            _timeTogglingView.SetIndicators(_paused, _timeSpeed);
        }
    }
}
