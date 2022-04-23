using System.Collections.Generic;
using UnityEngine;

namespace Units.Ancillaries.Fields
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(LineRenderer))]
    public class FieldVisualization : MonoBehaviour
    {
        [SerializeField] private float _oneDegreeMeshResolution;
        [SerializeField] private int _edgeResolveIterations;
        [SerializeField] private float _edgeDistanceThreshold;
        
        private static readonly Vector3 TargetPositionCorrection = Vector3.up * 1.5f;
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private LineRenderer _linesToTargets;
        
        private Mesh _viewMesh;

        private float _viewAngle;
        private float _viewRadius;
        private LayerMask _obstacleMask;
        private List<Transform> _targets;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _linesToTargets = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _viewMesh = new Mesh();
            _viewMesh.name = "View Mesh";
            _meshFilter.mesh = _viewMesh;
        }

        public void Show()
        {
            _meshRenderer.enabled = true;
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
            _linesToTargets.positionCount = 0;
        }

        public void DrawCircleSegment(float viewAngle, float viewRadius, LayerMask obstacleMask,
            List<Transform> targets)
        {
            _viewAngle = viewAngle;
            _viewRadius = viewRadius;
            _obstacleMask = obstacleMask;
            _targets = targets;
            
            var stepCount = Mathf.RoundToInt(_viewAngle * _oneDegreeMeshResolution);
            var stepAngleSize = _viewAngle / stepCount;

            var viewPoints = FindViewPoints(stepCount, stepAngleSize);

            RecalculateViewMesh(viewPoints);
            DrawLinesToTargets();
        }

        public void DrawCircle(float viewRadius, LayerMask obstacleMask, List<Transform> visibleTargets)
        {
            DrawCircleSegment(360f, viewRadius, obstacleMask, visibleTargets);
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
            if (_targets.Count == 0)
            {
                _linesToTargets.positionCount = 0;
            }
            else
            {
                _linesToTargets.positionCount = _targets.Count * 2;
                for (var i = 0; i < _targets.Count; i++)
                {
                    _linesToTargets.SetPosition(i * 2, _linesToTargets.transform.position);
                    _linesToTargets.SetPosition(i * 2 + 1,
                        _targets[i].transform.position + TargetPositionCorrection);
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
