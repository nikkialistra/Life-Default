using System;
using Environment.TimeCycle.TimeRegulation;
using Sirenix.OdinInspector;
using UI.Game.GameLook;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class TimeSpeedMultiplier : MonoBehaviour
    {
        [MinValue(1)]
        [SerializeField] private int _timeSpeedMultiplier = 3;

        private TimeToggling _timeToggling;
        
        private GameLookView _gameLookView;

        private PlayerInput _playerInput;

        private InputAction _pauseTimeAction;
        private InputAction _nextTimeSpeedAction;

        [Inject]
        public void Construct(TimeToggling timeToggling, GameLookView gameLookView, PlayerInput playerInput)
        {
            _timeToggling = timeToggling;
            _gameLookView = gameLookView;
            _playerInput = playerInput;
            
            _pauseTimeAction = _playerInput.actions.FindAction("Pause Time");
            _nextTimeSpeedAction = _playerInput.actions.FindAction("Next Time Speed");
        }

        private void OnEnable()
        {
            _pauseTimeAction.started += OnPauseTime;
            _nextTimeSpeedAction.started += OnNextTimeSpeed;
        }

        private void OnDisable()
        {
            _pauseTimeAction.started -= OnPauseTime;
            _nextTimeSpeedAction.started -= OnNextTimeSpeed;
        }

        private void OnPauseTime(InputAction.CallbackContext context)
        {
            ChangeTimeSpeedMultiplier();
        }
        
        private void OnNextTimeSpeed(InputAction.CallbackContext context)
        {
            ChangeTimeSpeedMultiplier();
        }
        
        private void ChangeTimeSpeedMultiplier()
        {
            if (Keyboard.current.ctrlKey.isPressed)
            {
                _timeToggling.TimeSpeedMultiplier = _timeSpeedMultiplier;
                _gameLookView.ToggleTimeSpeedMultipliedIndicator(true);
            }
            else
            {
                _timeToggling.TimeSpeedMultiplier = 1;
                _gameLookView.ToggleTimeSpeedMultipliedIndicator(false);
            }
        }
    }
}
