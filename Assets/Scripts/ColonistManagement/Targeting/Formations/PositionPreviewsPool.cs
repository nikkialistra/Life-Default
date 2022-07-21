using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ColonistManagement.Targeting.Formations
{
    public class PositionPreviewsPool : MonoBehaviour
    {
        public IReadOnlyList<PositionPreview> PositionPreviews => _positionPreviews;

        public float AnimationTime => _positionPreviewPrefab.AnimationTime;

        [Required]
        [SerializeField] private PositionPreview _positionPreviewPrefab;
        [Required]
        [SerializeField] private Transform _positionPreviewsParent;

        private FormationPreviewDrawing _formationPreviewDrawing;

        private readonly List<PositionPreview> _positionPreviews = new();

        private int _nextIndex = 0;

        public PositionPreview GetOrCreatePositionPreview(FormationColor formationColor)
        {
            PositionPreview positionPreview;

            if (_positionPreviews.Count <= _nextIndex)
            {
                positionPreview = Instantiate(_positionPreviewPrefab, _positionPreviewsParent);
                _positionPreviews.Add(positionPreview);
                positionPreview.Activate(formationColor);
            }
            else
            {
                positionPreview = _positionPreviews[_nextIndex];
                positionPreview.gameObject.SetActive(true);
                positionPreview.Activate(formationColor);
            }

            _nextIndex++;
            return positionPreview;
        }

        public void Animate()
        {
            for (int i = 0; i < _nextIndex; i++)
                _positionPreviews[i].StartAnimation();
        }

        public void FinishAnimation()
        {
            foreach (var positionPreview in _positionPreviews)
                positionPreview.gameObject.SetActive(false);
        }

        public void Reset()
        {
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
    }
}
