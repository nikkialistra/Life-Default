using Sirenix.OdinInspector;
using UnityEngine;

namespace Kernel.Selection
{
    [RequireComponent(typeof(Canvas))]
    public class SelectionArea : MonoBehaviour
    {
        [Title("Area")]
        [Required]
        [SerializeField] private RectTransform _selectionArea;
        
        [Title("Borders")]
        [Required]
        [SerializeField] private GameObject _bordersParent;
        [Space]
        [Required]
        [SerializeField] private RectTransform _topRect;
        [Required]
        [SerializeField] private RectTransform _leftRect;
        [Required]
        [SerializeField] private RectTransform _rightRect;
        [Required]
        [SerializeField] private RectTransform _bottomRect;

        private Canvas _canvas;

        private GameObject _selectionGameObject;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _selectionGameObject = _selectionArea.gameObject;
        }

        public void Draw(Rect rect)
        {
            if (_selectionGameObject.activeSelf == false)
            {
                _selectionGameObject.SetActive(true);
                _bordersParent.SetActive(true);
            }

            _selectionArea.position = rect.center;
            _selectionArea.sizeDelta = rect.size / _canvas.scaleFactor;

            DrawBorders(rect);
        }

        private void DrawBorders(Rect rect)
        {
            _topRect.position = new Vector2(rect.xMin + rect.width / 2, rect.yMax);
            _topRect.sizeDelta = new Vector2(rect.width / _canvas.scaleFactor, 1);
            
            _leftRect.position = new Vector2(rect.xMin, rect.yMin + rect.height / 2);
            _leftRect.sizeDelta = new Vector2(1, rect.height / _canvas.scaleFactor);

            _rightRect.position = new Vector2(rect.xMax, rect.yMin + rect.height / 2);
            _rightRect.sizeDelta = new Vector2(1, rect.height / _canvas.scaleFactor);

            _bottomRect.position = new Vector2(rect.xMin + rect.width / 2, rect.yMin);
            _bottomRect.sizeDelta = new Vector2(rect.width / _canvas.scaleFactor, 1);
        }

        public void StopDrawing()
        {
            _selectionGameObject.SetActive(false);
            _bordersParent.SetActive(false);
        }
    }
}