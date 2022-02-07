using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnitManagement.Targeting.Formations
{
    public class FormationPreviewDrawing : MonoBehaviour
    {
        [Required]
        [SerializeField] private PositionPreview _positionPreviewPrefab;
        [Required]
        [SerializeField] private Transform _positionPreviewsParent;

        private readonly List<PositionPreview> _positionPreviews = new();

        private Coroutine _flashFinishCoroutine;

        private int _nextIndex = 0;

        public void Show(IEnumerable<Vector3> formationPositions)
        {
            Reset();

            foreach (var formationPosition in formationPositions)
            {
                var positionPreview = GetOrCreatePositionPreview();
                positionPreview.transform.position = formationPosition;
            }
        }

        private void Reset()
        {
            if (_flashFinishCoroutine != null)
            {
                StopCoroutine(_flashFinishCoroutine);
            }

            foreach (var positionPreview in _positionPreviews)
            {
                if (positionPreview.Activated)
                {
                    positionPreview.Deactivate();
                    positionPreview.gameObject.SetActive(false);
                }
            }

            _nextIndex = 0;
        }

        private PositionPreview GetOrCreatePositionPreview()
        {
            PositionPreview positionPreview;

            if (_positionPreviews.Count <= _nextIndex)
            {
                positionPreview = Instantiate(_positionPreviewPrefab, _positionPreviewsParent);
                _positionPreviews.Add(positionPreview);
            }
            else
            {
                positionPreview = _positionPreviews[_nextIndex];
                positionPreview.gameObject.SetActive(true);
                positionPreview.Activate();
            }

            _nextIndex++;
            return positionPreview;
        }

        public void UpdatePositions(Vector3[] formationPositions)
        {
            for (var i = 0; i < formationPositions.Length; i++)
            {
                _positionPreviews[i].transform.position = formationPositions[i];
            }
        }

        public void Flash()
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                _positionPreviews[i].StartFlash();
            }

            _flashFinishCoroutine = StartCoroutine(FlashFinish(_positionPreviewPrefab.FadeTime));
        }

        private IEnumerator FlashFinish(float fadeTime)
        {
            yield return new WaitForSeconds(fadeTime);

            foreach (var positionPreview in _positionPreviews)
            {
                positionPreview.gameObject.SetActive(false);
            }
        }
    }
}
