using UnityEngine.InputSystem;

namespace Kernel.Utils
{
    public static class KeyboardExtensions
    {
        public static bool IsModifierKeyPressed(this Keyboard keyboard)
        {
            return keyboard.shiftKey.isPressed ||
                   keyboard.ctrlKey.isPressed ||
                   keyboard.altKey.isPressed;
        }
    }
}