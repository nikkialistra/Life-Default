using UnityEngine;

namespace UI
{
    public class GameCursors : MonoBehaviour
    {
        [SerializeField] private Texture2D _mainCursor;

        [SerializeField] private Texture2D _moveCursor;
        [SerializeField] private Texture2D _attackCursor;

        private void Awake()
        {
            SetDefaultCursor();
        }

        public void SetDefaultCursor()
        {
            Cursor.SetCursor(_mainCursor, Vector2.zero, CursorMode.Auto);
        }

        public void SetMoveCursor()
        {
            Cursor.SetCursor(_moveCursor, new Vector2(_moveCursor.width / 2, _moveCursor.height / 2), CursorMode.Auto);
        }

        public void SetAttackCursor()
        {
            Cursor.SetCursor(_attackCursor, new Vector2(_attackCursor.width / 2, _attackCursor.height / 2),
                CursorMode.Auto);
        }
    }
}
