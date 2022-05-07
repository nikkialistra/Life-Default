// Code based on https://forum.unity.com/threads/simple-runtime-terrain-editor.502650/

using System;
using System.Linq;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace General.Map
{
    public class TerrainModification : MonoBehaviour
    {
        [MinValue(0)]
        [SerializeField] private int _minDetailLayerNumber;
        [MinValue(0)]
        [SerializeField] private int _maxDetailLayerNumber;

        private int _brushWidth;
        private int _brushHeight;

        private float _strength = 0.01f;

        private Terrain _terrain;
        private LayerMask _terrainMask;

        private TerrainData _terrainData;
        private int _heightmapResolution;
        private int _detailResolution;
        private Vector3 _terrainSize;

        private float _sampledHeight;

        private Vector3 _originPositionCorrection;

        public enum ModificationType
        {
            Raise,
            Lower,
            Flatten,
            Sample,
            SampleAverage,
        }

        [Inject]
        public void Construct(Camera camera, Terrain terrain)
        {
            _terrain = terrain;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
        }

        private void Start()
        {
            _terrainData = _terrain.terrainData;
            _heightmapResolution = _terrainData.heightmapResolution;
            _detailResolution = _terrainData.detailResolution;
            _terrainSize = _terrainData.size;

            _originPositionCorrection = GlobalParameters.Instance.RaycastToTerrainCorrection;
        }

        [Button(ButtonSizes.Medium)]
        private void ClearDetailsAroundResources()
        {
            var resources = FindObjectsOfType<Resource>();

            foreach (var resource in resources)
            {
                resource.ClearDetailsAround();
            }
        }

        public void ClearDetailsAt(Vector3 position, int radius)
        {
            var closestTerrainPoint = GetClosestTerrainPoint(position);

            if (float.IsNegativeInfinity(closestTerrainPoint.x))
            {
                return;
            }

            CalculateDetailBrushArea(radius, closestTerrainPoint, out var detailBrushPosition, out var detailBrushSize);

            ClearDetailsAtLayers(detailBrushPosition, detailBrushSize);
        }

        private void CalculateDetailBrushArea(int radius, Vector3 closestTerrainPoint, out Vector2Int detailBrushPosition,
            out Vector2Int detailBrushSize)
        {
            var brushPosition = GetDetailBrushPosition(closestTerrainPoint, radius, radius);
            var brushSize = GetSafeDetailBrushSize(brushPosition.x, brushPosition.y, radius, radius);

            detailBrushPosition = brushPosition;
            detailBrushSize = brushSize;
        }

        private void ClearDetailsAtLayers(Vector2Int detailBrushPosition, Vector2Int detailBrushSize)
        {
            for (int layerNumber = _minDetailLayerNumber; layerNumber <= _maxDetailLayerNumber; layerNumber++)
            {
                var details = _terrainData.GetDetailLayer(detailBrushPosition.x, detailBrushPosition.y, detailBrushSize.x,
                    detailBrushSize.y, layerNumber);

                var circleRadius = detailBrushSize.y / 2;

                ClearInCircle(details, detailBrushSize, circleRadius);

                _terrainData.SetDetailLayer(detailBrushPosition.x, detailBrushPosition.y, layerNumber, details);
            }
        }

        private static void ClearInCircle(int[,] details, Vector2Int detailBrushSize, int circleRadius)
        {
            for (var y = 0; y < detailBrushSize.y; y++)
            {
                for (var x = 0; x < detailBrushSize.x; x++)
                {
                    if (InCircle(y, x, circleRadius))
                    {
                        details[y, x] = 0;
                    }
                }
            }
        }

        private static bool InCircle(int y, int x, int circleRadius)
        {
            return (Mathf.Pow(y - circleRadius, 2f) + Mathf.Pow(x - circleRadius, 2f) - float.Epsilon) <=
                   Mathf.Pow(circleRadius, 2f);
        }

        public void ModifyAt(Vector3 position, int radius, float strength, ModificationType modificationType)
        {
            var closestTerrainPoint = GetClosestTerrainPoint(position);

            if (float.IsNegativeInfinity(closestTerrainPoint.x))
            {
                return;
            }

            _brushWidth = radius;
            _brushHeight = radius;

            _strength = strength;

            switch (modificationType)
            {
                case ModificationType.Raise:
                    RaiseTerrain(closestTerrainPoint, _strength, _brushWidth, _brushHeight);
                    break;
                case ModificationType.Lower:
                    LowerTerrain(closestTerrainPoint, _strength, _brushWidth, _brushHeight);
                    break;
                case ModificationType.Flatten:
                    FlattenTerrain(closestTerrainPoint, _sampledHeight, _brushWidth, _brushHeight);
                    break;
                case ModificationType.Sample:
                    _sampledHeight = SampleHeight(closestTerrainPoint);
                    break;
                case ModificationType.SampleAverage:
                    _sampledHeight = SampleAverageHeight(closestTerrainPoint, _brushWidth, _brushHeight);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector3 GetClosestTerrainPoint(Vector3 position)
        {
            if (Physics.Raycast(new Ray(position + _originPositionCorrection, Vector3.down), out var hit,
                100f, _terrainMask))
            {
                return hit.point;
            }

            return Vector3.negativeInfinity;
        }

        private void RaiseTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var heights = _terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            for (var y = 0; y < brushSize.y; y++)
            {
                for (var x = 0; x < brushSize.x; x++)
                {
                    heights[y, x] += strength * Time.deltaTime;
                }
            }

            _terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
        }

        private void LowerTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var heights = _terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            for (var y = 0; y < brushSize.y; y++)
            {
                for (var x = 0; x < brushSize.x; x++)
                {
                    heights[y, x] -= strength * Time.deltaTime;
                }
            }

            _terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
        }

        private void FlattenTerrain(Vector3 worldPosition, float height, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var heights = _terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            for (var y = 0; y < brushSize.y; y++)
            {
                for (var x = 0; x < brushSize.x; x++)
                {
                    heights[y, x] = height;
                }
            }

            _terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
        }

        private float SampleHeight(Vector3 worldPosition)
        {
            var terrainPosition = WorldToTerrainPosition(worldPosition);

            return _terrainData.GetInterpolatedHeight((int)terrainPosition.x, (int)terrainPosition.z);
        }

        private float SampleAverageHeight(Vector3 worldPosition, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var heights2D = _terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            var heights = new float[heights2D.Length];

            var i = 0;

            for (int y = 0; y <= heights2D.GetUpperBound(0); y++)
            {
                for (int x = 0; x <= heights2D.GetUpperBound(1); x++)
                {
                    heights[i++] = heights2D[y, x];
                }
            }

            return heights.Average();
        }

        private Vector2Int GetBrushPosition(Vector3 worldPosition, int brushWidth, int brushHeight)
        {
            var terrainPosition = WorldToTerrainPosition(worldPosition);

            return new Vector2Int((int)Mathf.Clamp(terrainPosition.x - brushWidth / 2.0f, 0.0f, _heightmapResolution),
                (int)Mathf.Clamp(terrainPosition.z - brushHeight / 2.0f, 0.0f, _heightmapResolution));
        }
        
        private Vector2Int GetDetailBrushPosition(Vector3 worldPosition, int brushWidth, int brushHeight)
        {
            var terrainPosition = WorldToTerrainDetailPosition(worldPosition);

            return new Vector2Int((int)Mathf.Clamp(terrainPosition.x - brushWidth / 2.0f, 0.0f, _detailResolution),
                (int)Mathf.Clamp(terrainPosition.z - brushHeight / 2.0f, 0.0f, _detailResolution));
        }

        private Vector3 WorldToTerrainPosition(Vector3 worldPosition)
        {
            var terrainPosition = worldPosition - _terrain.GetPosition();

            terrainPosition = new Vector3(terrainPosition.x / _terrainSize.x, terrainPosition.y / _terrainSize.y,
                terrainPosition.z / _terrainSize.z);

            return new Vector3(terrainPosition.x * _heightmapResolution, 0, terrainPosition.z * _heightmapResolution);
        }

        private Vector3 WorldToTerrainDetailPosition(Vector3 worldPosition)
        {
            var terrainPosition = worldPosition - _terrain.GetPosition();

            terrainPosition = new Vector3(terrainPosition.x / _terrainSize.x, terrainPosition.y / _terrainSize.y,
                terrainPosition.z / _terrainSize.z);

            return new Vector3(terrainPosition.x * _terrainData.detailResolution, 0, terrainPosition.z * _terrainData.detailResolution);
        }

        private Vector2Int GetSafeBrushSize(int brushX, int brushY, int brushWidth, int brushHeight)
        {
            while (_heightmapResolution - (brushX + brushWidth) < 0)
            {
                brushWidth--;
            }

            while (_heightmapResolution - (brushY + brushHeight) < 0)
            {
                brushHeight--;
            }

            return new Vector2Int(brushWidth, brushHeight);
        }
        
        private Vector2Int GetSafeDetailBrushSize(int brushX, int brushY, int brushWidth, int brushHeight)
        {
            while (_detailResolution - (brushX + brushWidth) < 0)
            {
                brushWidth--;
            }

            while (_detailResolution - (brushY + brushHeight) < 0)
            {
                brushHeight--;
            }

            return new Vector2Int(brushWidth, brushHeight);
        }
    }
}
