using System.Collections;
using General;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Units.Ancillaries
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineToTrackedUnit : MonoBehaviour
    {
        [Required]
        [SerializeField] private Transform _unit;

        private LineRenderer _lineRenderer;

        private Transform _trackedUnitTransform;
        
        private Transform _linesToTrackedUnitsParent;
        
        private Coroutine _showingLineCoroutine;
        
        private Vector3 _lineToUnitTargetCorrection;

        [Inject]
        public void Construct(Transform linesToTrackedUnitsParent)
        {
            _linesToTrackedUnitsParent = linesToTrackedUnitsParent;
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _lineRenderer.transform.parent = _linesToTrackedUnitsParent;
            _lineRenderer.transform.position = Vector3.zero;

            _lineToUnitTargetCorrection = GlobalParameters.Instance.LineToUnitTargetCorrection;
        }

        public void ShowLineTo(Unit trackedUnit)
        {
            _lineRenderer.enabled = true;
            
            _trackedUnitTransform = trackedUnit.transform;

            if (_showingLineCoroutine != null)
            {
                StopCoroutine(_showingLineCoroutine);
            }
            
            _showingLineCoroutine = StartCoroutine(ShowingLine());
        }

        private IEnumerator ShowingLine()
        {
            while (true)
            {
                _lineRenderer.SetPosition(0, _unit.position + _lineToUnitTargetCorrection);
                _lineRenderer.SetPosition(1, _trackedUnitTransform.position + _lineToUnitTargetCorrection);

                yield return null;
            }
        }

        public void HideLine()
        {
            if (_showingLineCoroutine != null)
            {
                StopCoroutine(_showingLineCoroutine);
                _showingLineCoroutine = null;
            }

            _lineRenderer.enabled = false;
            _trackedUnitTransform = null;
        }
    }
}
