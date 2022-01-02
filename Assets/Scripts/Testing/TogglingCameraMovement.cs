using Cameras;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class TogglingCameraMovement : ITickable
    {
        private readonly CameraInputCombination _cameraInputCombination;

        private InputAction _toggleCameraMovementAction;

        public TogglingCameraMovement(CameraInputCombination cameraInputCombination)
        {
            _cameraInputCombination = cameraInputCombination;
        }

        public void Tick()
        {
            if (!Keyboard.current.altKey.isPressed)
            {
                return;
            }

            CheckForToggleCommand();
        }

        private void CheckForToggleCommand()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ToggleCameraMovement();
            }
        }

        private void ToggleCameraMovement()
        {
            _cameraInputCombination.enabled = !_cameraInputCombination.enabled;
        }
    }
}