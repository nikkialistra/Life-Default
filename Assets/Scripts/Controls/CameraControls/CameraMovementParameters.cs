using System;
using System.Collections.Generic;
using Saving;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controls.CameraControls
{
    public class CameraMovementParameters : MonoBehaviour
    {
        public float CameraSensitivity => _cameraSensitivity;
        public bool ScreenEdgeMouseScroll => _screenEdgeMouseScroll;

        private float _cameraSensitivity;
        private bool _screenEdgeMouseScroll;

        private readonly List<IDisposable> _disposables = new();

        [Inject]
        public void Construct(GameSettings gameSettings)
        {
            _cameraSensitivity = gameSettings.CameraSensitivity.Value;
            _screenEdgeMouseScroll = gameSettings.ScreenEdgeMouseScroll.Value;

            _disposables.Add(gameSettings.CameraSensitivity.Subscribe(value => _cameraSensitivity = value));
            _disposables.Add(gameSettings.ScreenEdgeMouseScroll.Subscribe(value => _screenEdgeMouseScroll = value));
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
