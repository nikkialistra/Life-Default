// Code based on https://forum.unity.com/threads/simple-runtime-terrain-editor.502650/

using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace General.Map
{
    public class TerrainModification : MonoBehaviour
    {
        [SerializeField] private int _brushWidth = 8;
        [SerializeField] private int _brushHeight = 8;

        [Range(0.001f, 0.1f)]
        [SerializeField] private float _strength = 0.01f;
        [SerializeField] private int _layer;
        

        private Camera _camera;
        
        private Terrain _terrain;
        private TerrainCollider _terrainCollider;

        private LayerMask _terrainMask;

        private float _sampledHeight;

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
            _camera = camera;
            
            _terrain = terrain;
            _terrainCollider = terrain.GetComponent<TerrainCollider>();
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
        }

        public void DeleteDetailsAt(Vector3 position, int radius)
        {
            var closestTerrainPoint = GetClosestTerrainPoint(position);

            if (float.IsNegativeInfinity(closestTerrainPoint.x))
            {
                return;
            }
            
            _brushWidth = radius;
            _brushHeight = radius;
            
            var divider = _terrain.terrainData.heightmapResolution / _terrain.terrainData.detailResolution;
            
            var brushPosition = GetBrushPosition(closestTerrainPoint, _brushWidth, _brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, _brushWidth, _brushHeight);

            var detailBrushPosition = brushPosition / divider;
            var detailBrushSize = brushSize / divider;

            var terrainData = GetTerrainData();

            var details = terrainData.GetDetailLayer(detailBrushPosition.x, detailBrushPosition.y, detailBrushSize.x, detailBrushSize.y, _layer);

            for (var y = 0; y < detailBrushSize.y; y++)
            {
                for (var x = 0; x < detailBrushSize.x; x++)
                {
                    if (details[y, x] != 0)
                    {
                        Debug.Log($"{x}:{y} - {details[y, x]}");
                    }

                    details[y, x] = 0;
                }
            }

            terrainData.SetDetailLayer(detailBrushPosition.x, detailBrushPosition.y, _layer, details);
        }

        public void ModifyAt(Vector3 position, int radius, ModificationType modificationType)
        {
            var closestTerrainPoint = GetClosestTerrainPoint(position);

            if (float.IsNegativeInfinity(closestTerrainPoint.x))
            {
                return;
            }

            _brushWidth = radius;
            _brushHeight = radius;

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
            if (Physics.Raycast(new Ray(position, Vector3.down), out var hit,
                float.PositiveInfinity, _terrainMask))
            {
                return hit.point;
            }
            else
            {
                return Vector3.negativeInfinity;
            }
        }

        private void RaiseTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var terrainData = GetTerrainData();

            var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            for (var y = 0; y < brushSize.y; y++)
            {
                for (var x = 0; x < brushSize.x; x++)
                {
                    heights[y, x] += strength * Time.deltaTime;
                }
            }

            terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
        }

        private void LowerTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var terrainData = GetTerrainData();

            var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            for (var y = 0; y < brushSize.y; y++)
            {
                for (var x = 0; x < brushSize.x; x++)
                {
                    heights[y, x] -= strength * Time.deltaTime;
                }
            }

            terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
        }

        private void FlattenTerrain(Vector3 worldPosition, float height, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var terrainData = GetTerrainData();

            var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

            for (var y = 0; y < brushSize.y; y++)
            {
                for (var x = 0; x < brushSize.x; x++)
                {
                    heights[y, x] = height;
                }
            }

            terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
        }

        private float SampleHeight(Vector3 worldPosition)
        {
            var terrainPosition = WorldToTerrainPosition(worldPosition);

            return GetTerrainData().GetInterpolatedHeight((int)terrainPosition.x, (int)terrainPosition.z);
        }

        private float SampleAverageHeight(Vector3 worldPosition, int brushWidth, int brushHeight)
        {
            var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);
            var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

            var heights2D = GetTerrainData().GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

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

            var heightmapResolution = GetHeightmapResolution();

            return new Vector2Int((int)Mathf.Clamp(terrainPosition.x - brushWidth / 2.0f, 0.0f, heightmapResolution),
                (int)Mathf.Clamp(terrainPosition.z - brushHeight / 2.0f, 0.0f, heightmapResolution));
        }

        private Vector3 WorldToTerrainPosition(Vector3 worldPosition)
        {
            var terrainPosition = worldPosition - _terrain.GetPosition();

            var terrainSize = GetTerrainSize();

            var heightmapResolution = GetHeightmapResolution();

            terrainPosition = new Vector3(terrainPosition.x / terrainSize.x, terrainPosition.y / terrainSize.y,
                terrainPosition.z / terrainSize.z);

            return new Vector3(terrainPosition.x * heightmapResolution, 0, terrainPosition.z * heightmapResolution);
        }

        private Vector2Int GetSafeBrushSize(int brushX, int brushY, int brushWidth, int brushHeight)
        {
            var heightmapResolution = GetHeightmapResolution();

            while (heightmapResolution - (brushX + brushWidth) < 0) brushWidth--;

            while (heightmapResolution - (brushY + brushHeight) < 0) brushHeight--;

            return new Vector2Int(brushWidth, brushHeight);
        }
        
        private TerrainData GetTerrainData()
        {
            return _terrain.terrainData;
        }

        private int GetHeightmapResolution()
        {
            return GetTerrainData().heightmapResolution;
        }

        private Vector3 GetTerrainSize()
        {
            return GetTerrainData().size;
        }
    }
}
