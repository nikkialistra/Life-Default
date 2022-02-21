using System;
using UI.Game.GameLook.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public class TimeToggling : MonoBehaviour
    {
        private bool _paused;
        private TimeSpeed _timeSpeed = TimeSpeed.X1;

        private TimeTogglingView _timeTogglingView;
        private PlayerInput _playerInput;

        private InputAction _pauseTimeAction;
        private InputAction _nextTimeSpeedAction;

        [Inject]
        public void Construct(TimeTogglingView timeTogglingView, PlayerInput playerInput)
        {
            _timeTogglingView = timeTogglingView;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _pauseTimeAction = _playerInput.actions.FindAction("Pause Time");
            _nextTimeSpeedAction = _playerInput.actions.FindAction("Next Time Speed");
        }

        public event Action<bool> PauseChange;

        private void Start()
        {
            Toggle();
        }

        private void OnEnable()
        {
            _pauseTimeAction.started += PauseTime;
            _nextTimeSpeedAction.started += NextTimeSpeed;

            _timeTogglingView.Pause += OnPause;
            _timeTogglingView.X1 += OnX1;
            _timeTogglingView.X2 += OnX2;
            _timeTogglingView.X3 += OnX3;
        }

        private void OnDisable()
        {
            _pauseTimeAction.started -= PauseTime;
            _nextTimeSpeedAction.started -= NextTimeSpeed;

            _timeTogglingView.Pause -= OnPause;
            _timeTogglingView.X1 -= OnX1;
            _timeTogglingView.X2 -= OnX2;
            _timeTogglingView.X3 -= OnX3;
        }

        public void Toggle()
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
                };
            }

            PauseChange?.Invoke(_paused);
        }

        private void PauseTime(InputAction.CallbackContext context)
        {
            _paused = !_paused;
            UpdateView();
            Toggle();
        }

        private void NextTimeSpeed(InputAction.CallbackContext context)
        {
            _timeSpeed = _timeSpeed switch
            {
                TimeSpeed.X1 => TimeSpeed.X2,
                TimeSpeed.X2 => TimeSpeed.X3,
                TimeSpeed.X3 => TimeSpeed.X1,
                _ => throw new ArgumentOutOfRangeException()
            };

            UpdateView();
            Toggle();
        }

        private void UpdateView()
        {
            _timeTogglingView.SetIndicators(_paused, _timeSpeed);
        }

        private void OnPause(bool value)
        {
            _paused = value;
            Toggle();
        }

        private void OnX1()
        {
            _timeSpeed = TimeSpeed.X1;
            Toggle();
        }

        private void OnX2()
        {
            _timeSpeed = TimeSpeed.X2;
            Toggle();
        }

        private void OnX3()
        {
            _timeSpeed = TimeSpeed.X3;
            Toggle();
        }
    }
}
