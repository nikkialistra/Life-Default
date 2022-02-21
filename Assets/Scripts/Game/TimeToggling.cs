using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Game
{
    public class TimeToggling : MonoBehaviour
    {
        private bool _paused;
        private float _timeSpeed = 1f;

        private TimeTogglingView _timeTogglingView;

        [Inject]
        public void Construct(TimeTogglingView timeTogglingView)
        {
            _timeTogglingView = timeTogglingView;
        }

        private void Start()
        {
            Toggle();
        }

        private void OnEnable()
        {
            _timeTogglingView.Pause += Pause;
            _timeTogglingView.X1 += X1;
            _timeTogglingView.X2 += X2;
            _timeTogglingView.X3 += X3;
        }

        public void Toggle()
        {
            if (_paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = _timeSpeed;
            }
        }

        private void Pause(bool value)
        {
            _paused = value;
            Toggle();
        }

        private void X1()
        {
            _timeSpeed = 1f;
            Toggle();
        }

        private void X2()
        {
            _timeSpeed = 2f;
            Toggle();
        }

        private void X3()
        {
            _timeSpeed = 3f;
            Toggle();
        }
    }
}
