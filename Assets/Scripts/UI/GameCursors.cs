using UnityEngine;

namespace UI
{
    public class GameCursors : MonoBehaviour
    {
        [SerializeField] private Texture2D _mainCursor;

        [SerializeField] private Texture2D _targetingCursor;


        private void Awake()
        {
            SetDefaultCursor();
        }

        public void SetTargetingCursor()
        {
            Cursor.SetCursor(_targetingCursor, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        }

        public void SetDefaultCursor()
        {
            Cursor.SetCursor(_mainCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}
