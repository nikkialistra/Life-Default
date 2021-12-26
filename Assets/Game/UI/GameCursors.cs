using UnityEngine;

namespace Game.UI
{
    public class GameCursors : MonoBehaviour
    {
        [SerializeField] private Texture2D _mainCursor;

        private void Awake()
        {
            Cursor.SetCursor(_mainCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}