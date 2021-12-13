using UnityEngine;

namespace Kernel.Selection
{
    public class SelectionArea
    {
        private readonly RectTransform _selectionRect;
        private readonly GameObject _selectionGameObject;
        private readonly Canvas _canvas;

        public SelectionArea(RectTransform selectionRect, Canvas canvas)
        {
            _selectionRect = selectionRect;
            _selectionGameObject = selectionRect.gameObject;
            _canvas = canvas;
        }

        public void Draw(Rect rect)
        {
            if (_selectionGameObject.activeSelf == false)
                _selectionGameObject.SetActive(true);
            
            _selectionRect.position = rect.center;
            _selectionRect.sizeDelta = rect.size / _canvas.scaleFactor;
        }

        public void StopDrawing() => _selectionGameObject.SetActive(false);
    }
}