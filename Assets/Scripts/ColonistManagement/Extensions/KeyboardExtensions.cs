using UnityEngine.InputSystem;

namespace ColonistManagement.Extensions
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
