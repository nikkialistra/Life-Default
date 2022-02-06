using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace UnitManagement.Targeting.Formations
{
    public class FormationPreviewDrawing : MonoBehaviour
    {
        [Required]
        [SerializeField] private PositionPreview _positionPreviewPrefab;
        [Required]
        [SerializeField] private Transform _positionPreviewsParent;

        private ObjectPool<PositionPreview> _positionPreviewPool;

        private List<PositionPreview> _shownPositionPreviews;

        private Coroutine _flashPositionPreviewsCoroutine;

        private void Start()
        {
            _positionPreviewPool =
                new ObjectPool<PositionPreview>(CreatePositionPreview, OnTakePositionPreview, OnReturnPositionPreview);
        }

        public void Show(IEnumerable<Vector3> formationPositions)
        {
            _shownPositionPreviews = new List<PositionPreview>();

            foreach (var formationPosition in formationPositions)
            {
                var positionPreview = _positionPreviewPool.Get();
                positionPreview.transform.position = formationPosition;

                _shownPositionPreviews.Add(positionPreview);
            }
        }

        public void UpdatePositions(Vector3[] formationPositions)
        {
            for (var i = 0; i < formationPositions.Length; i++)
            {
                _shownPositionPreviews[i].transform.position = formationPositions[i];
            }
        }

        public void Flash()
        {
            foreach (var positionPreview in _shownPositionPreviews)
            {
                positionPreview.StartFlash(OnFlashFinish);
            }
        }

        private void OnFlashFinish(PositionPreview positionPreview)
        {
            _positionPreviewPool.Release(positionPreview);
        }

        private PositionPreview CreatePositionPreview()
        {
            var positionPreview = Instantiate(_positionPreviewPrefab, _positionPreviewsParent);

            return positionPreview;
        }

        private static void OnTakePositionPreview(PositionPreview positionPreview)
        {
            positionPreview.gameObject.SetActive(true);
        }

        private static void OnReturnPositionPreview(PositionPreview positionPreview)
        {
            positionPreview.gameObject.SetActive(false);
        }
    }
}
