using System;
using General.TimeCycle.TimeRegulation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components
{
    public class TimeTogglingView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/TimeToggling";

        private Toggle _pause;
        private Toggle _x1;
        private Toggle _x2;
        private Toggle _x3;

        private TimeToggling _timeToggling;
        
        private PlayerInput _playerInput;

        private InputAction _pauseTimeAction;
        private InputAction _nextTimeSpeedAction;

        [Inject]
        public void Construct(TimeToggling timeToggling, PlayerInput playerInput)
        {
            _timeToggling = timeToggling;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _pause = Tree.Q<Toggle>("pause");
            _x1 = Tree.Q<Toggle>("x1");
            _x2 = Tree.Q<Toggle>("x2");
            _x3 = Tree.Q<Toggle>("x3");
            
            _pauseTimeAction = _playerInput.actions.FindAction("Pause Time");
            _nextTimeSpeedAction = _playerInput.actions.FindAction("Next Time Speed");
        }

        public VisualElement Tree { get; private set; }

        private void Start()
        {
            _x1.SetValueWithoutNotify(true);
        }

        private void OnEnable()
        {
            _pause.RegisterValueChangedCallback(OnPauseToggle);
            _x1.RegisterValueChangedCallback(OnX1Toggle);
            _x2.RegisterValueChangedCallback(OnX2Toggle);
            _x3.RegisterValueChangedCallback(OnX3Toggle);
            
            _pauseTimeAction.started += OnPauseTime;
            _nextTimeSpeedAction.started += OnNextTimeSpeed;
        }

        private void OnDisable()
        {
            _pause.UnregisterValueChangedCallback(OnPauseToggle);
            _x1.UnregisterValueChangedCallback(OnX1Toggle);
            _x2.UnregisterValueChangedCallback(OnX2Toggle);
            _x3.UnregisterValueChangedCallback(OnX3Toggle);
            
            _pauseTimeAction.started -= OnPauseTime;
            _nextTimeSpeedAction.started -= OnNextTimeSpeed;
        }

        public void SetIndicators(bool paused, TimeSpeed timeSpeed)
        {
            _pause.SetValueWithoutNotify(paused);
            switch (timeSpeed)
            {
                case TimeSpeed.X1:
                    CheckX1Toggle();
                    break;
                case TimeSpeed.X2:
                    CheckX2Toggle();
                    break;
                case TimeSpeed.X3:
                    CheckX3Toggle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeSpeed), timeSpeed, null);
            }
        }

        private void OnPauseToggle(ChangeEvent<bool> _)
        {
            _timeToggling.Pause();
        }

        private void OnX1Toggle(ChangeEvent<bool> _)
        {
            _timeToggling.ChangeSpeed(TimeSpeed.X1);
        }

        private void OnX2Toggle(ChangeEvent<bool> _)
        {
            _timeToggling.ChangeSpeed(TimeSpeed.X2);
        }

        private void OnX3Toggle(ChangeEvent<bool> _)
        {
            _timeToggling.ChangeSpeed(TimeSpeed.X3);
        }

        private void OnPauseTime(InputAction.CallbackContext context)
        {
            _timeToggling.Pause();
        }

        private void OnNextTimeSpeed(InputAction.CallbackContext obj)
        {
            _timeToggling.NextSpeed();
        }

        private void CheckX1Toggle()
        {
            UncheckToggles();
            _x1.SetValueWithoutNotify(true);
        }

        private void CheckX2Toggle()
        {
            UncheckToggles();
            _x2.SetValueWithoutNotify(true);
        }

        private void CheckX3Toggle()
        {
            UncheckToggles();
            _x3.SetValueWithoutNotify(true);
        }

        private void UncheckToggles()
        {
            _x1.SetValueWithoutNotify(false);
            _x2.SetValueWithoutNotify(false);
            _x3.SetValueWithoutNotify(false);
        }
    }
}
