using System.Collections;
using Infrastructure.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Units.Ancillaries
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineToTrackedUnit : MonoBehaviour
    {
        [Required]
        [SerializeField] private Transform _unitTransform;
        [Space]
        [SerializeField] private float _distanceForOneSegment = 3f;
        [Space]
        [SerializeField] private Vector3 _lineToUnitTargetCorrection = Vector3.up * 0.4f;
        [SerializeField] private float _updateTime = 0.2f;

        private LineRenderer _lineRenderer;

        private Transform _trackedUnitTransform;
        private Transform _linesToTrackedUnitsParent;

        private Coroutine _showingLineCoroutine;

        private Vector3 _raycastToTerrainCorrection;

        private int _terrainMask;

        [Inject]
        public void Construct(Transform linesToTrackedUnitsParent, RaycastingSettings raycastingSettings)
        {
            _linesToTrackedUnitsParent = linesToTrackedUnitsParent;

            _raycastToTerrainCorrection = raycastingSettings.RaycastToTerrainCorrection;
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            _terrainMask = LayerMask.GetMask("Terrain");
        }

        private void Start()
        {
            _lineRenderer.transform.parent = _linesToTrackedUnitsParent;
            _lineRenderer.transform.position = Vector3.zero;
        }

        public void ShowLineTo(Unit trackedUnit)
        {
            _lineRenderer.enabled = true;

            _trackedUnitTransform = trackedUnit.transform;

            if (_showingLineCoroutine != null)
                StopCoroutine(_showingLineCoroutine);

            _showingLineCoroutine = StartCoroutine(CShowingLine());
        }

        private IEnumerator CShowingLine()
        {
            while (true)
            {
                CalculateLineRendererPositions();

                yield return new WaitForSeconds(_updateTime);
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

        private void CalculateLineRendererPositions()
        {
            var distance = Vector3.Distance(_unitTransform.position, _trackedUnitTransform.position);
            var numberOfSegments = (int)(distance / _distanceForOneSegment);

            _lineRenderer.positionCount = numberOfSegments + 2;

            _lineRenderer.SetPosition(0, _unitTransform.position + _lineToUnitTargetCorrection);

            for (int i = 1; i <= numberOfSegments; i++)
                _lineRenderer.SetPosition(i, CalculatePositionAt(i));

            _lineRenderer.SetPosition(numberOfSegments + 1,
                _trackedUnitTransform.position + _lineToUnitTargetCorrection);
        }

        private Vector3 CalculatePositionAt(int index)
        {
            var vectorFraction = (_trackedUnitTransform.position - _unitTransform.position) *
                                 ((float)index / _lineRenderer.positionCount);

            var position = _unitTransform.position + vectorFraction;

            return GetLandPositionAt(position) + _lineToUnitTargetCorrection;
        }

        private Vector3 GetLandPositionAt(Vector3 position)
        {
            if (Physics.Raycast(new Ray(position + _raycastToTerrainCorrection, Vector3.down), out var hit,
                100f, _terrainMask))
                return hit.point;
            else
                return Vector3.zero;
        }
    }
}
