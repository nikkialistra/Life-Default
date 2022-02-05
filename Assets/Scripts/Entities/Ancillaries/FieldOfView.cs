using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Ancillaries
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(LineRenderer))]
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private float _recalculationTime = 0.2f;

        [Space]
        [SerializeField] private float _viewRadius;
        [Range(0, 360)]
        [SerializeField] private float _viewAngle;

        [Title("Masks")]
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstacleMask;

        [Title("Debug")]
        [SerializeField] private float _oneDegreeMeshResolution;
        [SerializeField] private int _edgeResolveIterations;
        [SerializeField] private float _edgeDistanceThreshold;

        private static readonly Vector3 TargetPositionCorrection = Vector3.up * 1.5f;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private LineRenderer _linesToTargets;

        private readonly List<Transform> _visibleTargets = new();

        private Mesh _viewMesh;

        private bool _showFieldOfView;
        private float _updateTime;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _linesToTargets = GetComponent<LineRenderer>();
        }

        public IEnumerable<Transform> VisibleTargets => _visibleTargets;

        private void Start()
        {
            _viewMesh = new Mesh();
            _viewMesh.name = "View Mesh";
            _meshFilter.mesh = _viewMesh;
        }

        private void Update()
        {
            if (Time.time > _updateTime)
            {
                _updateTime += _recalculationTime;
                UpdateFieldOfView();
            }
        }

        [Button(ButtonSizes.Large)]
        private void ToggleDebugShow()
        {
            _showFieldOfView = !_showFieldOfView;

            if (_showFieldOfView)
            {
                _meshRenderer.enabled = true;
            }
            else
            {
                _meshRenderer.enabled = false;
                _linesToTargets.positionCount = 0;
            }
        }

        private void UpdateFieldOfView()
        {
            FindVisibleTargets();

            if (_showFieldOfView)
            {
                DrawFieldOfView();
            }
        }

        private void FindVisibleTargets()
        {
            _visibleTargets.Clear();
            var targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

            foreach (var target in targetsInViewRadius)
            {
                var targetPosition = target.transform.position + TargetPositionCorrection;
                var directionToTarget = (targetPosition - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle / 2)
                {
                    var distanceToTarget = Vector3.Distance(transform.position, targetPosition);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        _visibleTargets.Add(target.transform);
                    }
                }
            }
        }

        private void DrawFieldOfView()
        {
            var stepCount = Mathf.RoundToInt(_viewAngle * _oneDegreeMeshResolution);
            var stepAngleSize = _viewAngle / stepCount;

            var viewPoints = FindViewPoints(stepCount, stepAngleSize);

            RecalculateViewMesh(viewPoints);
            DrawLinesToTargets();
        }

        private List<Vector3> FindViewPoints(int stepCount, float stepAngleSize)
        {
            var viewPoints = new List<Vector3>();
            var oldViewCast = new ViewCastInfo();

            for (var i = 0; i <= stepCount; i++)
            {
                var angle = transform.eulerAngles.y - _viewAngle / 2 + stepAngleSize * i;
                var newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    var edgeDistanceThresholdExceeded =
                        Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > _edgeDistanceThreshold;
                    if (oldViewCast.Hit != newViewCast.Hit ||
                        (oldViewCast.Hit && newViewCast.Hit && edgeDistanceThresholdExceeded))
                    {
                        var edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.PointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointA);
                        }

                        if (edge.PointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointB);
                        }
                    }
                }

                viewPoints.Add(newViewCast.Point);
                oldViewCast = newViewCast;
            }

            return viewPoints;
        }

        private void RecalculateViewMesh(List<Vector3> viewPoints)
        {
            var vertexCount = viewPoints.Count + 1;
            var vertices = new Vector3[vertexCount];
            var triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (var i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            _viewMesh.Clear();

            _viewMesh.vertices = vertices;
            _viewMesh.triangles = triangles;
            _viewMesh.RecalculateNormals();
        }

        private void DrawLinesToTargets()
        {
            if (_visibleTargets.Count == 0)
            {
                _linesToTargets.positionCount = 0;
            }
            else
            {
                _linesToTargets.positionCount = _visibleTargets.Count * 2;
                for (var i = 0; i < _visibleTargets.Count; i++)
                {
                    _linesToTargets.SetPosition(i * 2, _linesToTargets.transform.position);
                    _linesToTargets.SetPosition(i * 2 + 1,
                        _visibleTargets[i].transform.position + TargetPositionCorrection);
                }
            }
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            var minAngle = minViewCast.Angle;
            var maxAngle = maxViewCast.Angle;
            var minPoint = Vector3.zero;
            var maxPoint = Vector3.zero;

            for (var i = 0; i < _edgeResolveIterations; i++)
            {
                var angle = (minAngle + maxAngle) / 2;
                var newViewCast = ViewCast(angle);

                var edgeDstThresholdExceeded =
                    Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > _edgeDistanceThreshold;
                if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.Point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            var direction = DirectionFromAngle(globalAngle);

            if (Physics.Raycast(transform.position, direction, out var hit, _viewRadius, _obstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + direction * _viewRadius, _viewRadius,
                    globalAngle);
            }
        }

        private static Vector3 DirectionFromAngle(float angleInDegrees)
        {
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private readonly struct ViewCastInfo
        {
            public readonly bool Hit;
            public readonly Vector3 Point;
            public readonly float Distance;
            public readonly float Angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                Hit = hit;
                Point = point;
                Distance = distance;
                Angle = angle;
            }
        }

        private readonly struct EdgeInfo
        {
            public readonly Vector3 PointA;
            public readonly Vector3 PointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }
    }
}
