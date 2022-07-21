using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ColonistManagement.Targeting.Formations
{
    [RequireComponent(typeof(PositionPreviewsPool))]
    public class FormationPreviewDrawing : MonoBehaviour
    {
        [Title("Special Previews")]
        [Required]
        [SerializeField] private PositionPreview _directionArrow;
        [Required]
        [SerializeField] private PositionPreview _noFormationMark;

        private Coroutine _flashFinishCoroutine;

        private bool _showNoFormationMark;

        private FormationColor _formationColor = FormationColor.White;

        private PositionPreviewsPool _positionPreviewsPool;

        public bool ShowDirectionArrow { get; set; }

        private void Awake()
        {
            _positionPreviewsPool = GetComponent<PositionPreviewsPool>();
        }

        public void ChangeColor(FormationColor formationColor)
        {
            _formationColor = formationColor;
        }

        public void Show(Vector3[] formationPositions, float rotation)
        {
            Reset();

            int startIndex;

            if (ShowDirectionArrow)
            {
                PlaceDirectionArrow(formationPositions[0], rotation);
                startIndex = 1;
            }
            else
            {
                startIndex = 0;
            }

            while (startIndex < formationPositions.Length)
            {
                var positionPreview = _positionPreviewsPool.GetOrCreatePositionPreview(_formationColor);
                positionPreview.transform.position = formationPositions[startIndex];

                startIndex++;
            }
        }

        public void ShowNoFormationMark(Vector3 position)
        {
            Reset();

            _showNoFormationMark = true;

            _noFormationMark.gameObject.SetActive(true);
            _noFormationMark.Activate(_formationColor);
            _noFormationMark.transform.position = position;
        }

        public void UpdatePositions(Vector3[] formationPositions, float rotation)
        {
            if (ShowDirectionArrow)
            {
                UpdateDirectionArrow(formationPositions[0], rotation);

                for (int i = 1; i < formationPositions.Length; i++)
                    _positionPreviewsPool.PositionPreviews[i - 1].transform.position = formationPositions[i];
            }
            else
            {
                for (int i = 0; i < formationPositions.Length; i++)
                    _positionPreviewsPool.PositionPreviews[i].transform.position = formationPositions[i];
            }
        }

        public void Animate()
        {
            if (_showNoFormationMark)
                _noFormationMark.StartAnimation();
            else
                AnimateFormation();

            _flashFinishCoroutine = StartCoroutine(CFinishAnimation(_positionPreviewsPool.AnimationTime));
        }

        public void Reset()
        {
            _showNoFormationMark = false;
            _noFormationMark.gameObject.SetActive(false);

            if (_flashFinishCoroutine != null)
            {
                StopCoroutine(_flashFinishCoroutine);
                _flashFinishCoroutine = null;
            }

            if (_directionArrow.Activated)
            {
                _directionArrow.Deactivate();
                _directionArrow.gameObject.SetActive(false);
            }

            _positionPreviewsPool.Reset();
        }

        private void AnimateFormation()
        {
            if (ShowDirectionArrow)
                _directionArrow.StartAnimation();

            _positionPreviewsPool.Animate();
        }

        private void PlaceDirectionArrow(Vector3 directionArrowPosition, float rotation)
        {
            _directionArrow.gameObject.SetActive(true);
            _directionArrow.Activate(_formationColor);
            _directionArrow.transform.position = directionArrowPosition;
            _directionArrow.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        private void UpdateDirectionArrow(Vector3 directionArrowPosition, float rotation)
        {
            _directionArrow.transform.position = directionArrowPosition;
            _directionArrow.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        private IEnumerator CFinishAnimation(float fadeTime)
        {
            yield return new WaitForSeconds(fadeTime);

            _noFormationMark.gameObject.SetActive(false);
            _directionArrow.gameObject.SetActive(false);

            _positionPreviewsPool.FinishAnimation();
        }
    }
}
