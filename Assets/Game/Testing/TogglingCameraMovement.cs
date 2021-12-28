using System;
using Game.Cameras;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Testing
{
    public class TogglingCameraMovement : IInitializable, IDisposable
    {
        private readonly CameraInputCombination _cameraInputCombination;
        
        private readonly PlayerInput _playerInput;

        private InputAction _toggleCameraMovementAction;

        public TogglingCameraMovement(CameraInputCombination cameraInputCombination, PlayerInput playerInput)
        {
            _cameraInputCombination = cameraInputCombination;
            _playerInput = playerInput;
        }

        public void Initialize()
        {
            _toggleCameraMovementAction = _playerInput.actions.FindAction("TestingToggleCameraMovement");
            _toggleCameraMovementAction.started += ToggleCameraMovement;
        }

        public void Dispose()
        {
            _toggleCameraMovementAction.started -= ToggleCameraMovement;
        }

        private void ToggleCameraMovement(InputAction.CallbackContext context)
        {
            _cameraInputCombination.enabled = !_cameraInputCombination.enabled;
        }
    }
}