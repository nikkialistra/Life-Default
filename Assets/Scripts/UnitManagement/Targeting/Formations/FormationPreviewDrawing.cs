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

        private Coroutine _flashPositionPreviewsCoroutine;

        private void Start()
        {
            _positionPreviewPool =
                new ObjectPool<PositionPreview>(CreatePositionPreview, OnTakePositionPreview, OnReturnPositionPreview);
        }

        public void Show(IEnumerable<Vector3> formationPositions)
        {
            foreach (var formationPosition in formationPositions)
            {
                var positionPreview = _positionPreviewPool.Get();
                positionPreview.transform.position = formationPosition;

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
