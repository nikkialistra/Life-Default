using System;
using Game;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TimeTogglingView : MonoBehaviour
    {
        private Toggle _pause;
        private Toggle _x1;
        private Toggle _x2;
        private Toggle _x3;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>("UI/Markup/GameLook/Components/TimeToggling").CloneTree();

            _pause = Tree.Q<Toggle>("pause");
            _x1 = Tree.Q<Toggle>("x1");
            _x2 = Tree.Q<Toggle>("x2");
            _x3 = Tree.Q<Toggle>("x3");
        }

        public VisualElement Tree { get; private set; }

        public event Action<bool> Pause;
        public event Action X1;
        public event Action X2;
        public event Action X3;

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
        }

        private void OnDisable()
        {
            _pause.UnregisterValueChangedCallback(OnPauseToggle);
            _x1.UnregisterValueChangedCallback(OnX1Toggle);
            _x2.UnregisterValueChangedCallback(OnX2Toggle);
            _x3.UnregisterValueChangedCallback(OnX3Toggle);
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
            Pause?.Invoke(_pause.value);
        }

        private void OnX1Toggle(ChangeEvent<bool> _)
        {
            CheckX1Toggle();

            X1?.Invoke();
        }

        private void OnX2Toggle(ChangeEvent<bool> _)
        {
            CheckX2Toggle();

            X2?.Invoke();
        }

        private void OnX3Toggle(ChangeEvent<bool> _)
        {
            CheckX3Toggle();

            X3?.Invoke();
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
