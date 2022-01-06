using System;
using System.Collections.Generic;
using MapGeneration.Data;
using MapGeneration.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace MapGeneration.Generators
{
    public class TerrainGenerator : MonoBehaviour
    {
        [Title("Terrain Settings")]
        [SerializeField] private int _colliderLODIndex;
        [SerializeField] private LODInfo[] _detailLevels;

        [SerializeField] private MeshSettings _meshSettings;
        [SerializeField] private HeightMapSettings _heightMapSettings;

        [SerializeField] private Transform _viewer;
        [SerializeField] private Material _mapMaterial;

        [Title("Other")]
        [SerializeField] private bool _loadAsync;

        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float SqrViewerMoveThresholdForChunkUpdate =
            ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

        private Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;

        private float _meshWorldSize;
        private int _chunksVisibleInViewDst;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new();
        private readonly List<TerrainChunk> _visibleTerrainChunks = new();

        private bool _chunkGenerated;

        [Inject]
        public void Construct(Camera camera)
        {
            _viewer = camera.transform;
        }

        public event Action ChunkGenerated;

        public IEnumerable<TerrainChunk> TerrainChunks => _terrainChunkDictionary.Values;

        private void Update()
        {
            _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

            if (_viewerPosition != _viewerPositionOld)
            {
                foreach (var chunk in _visibleTerrainChunks)
                {
                    chunk.UpdateCollisionMesh();
                }
            }

            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate)
            {
                _viewerPositionOld = _viewerPosition;
                UpdateVisibleChunks();
            }
        }

        private void OnDestroy()
        {
            foreach (var terrainChunk in TerrainChunks)
            {
                terrainChunk.VisibilityChange -= OnTerrainChunkVisibilityChange;
            }
        }

        public void Generate()
        {
            var maxViewDst = _detailLevels[^1].VisibleDistanceThreshold;
            _meshWorldSize = _meshSettings.MeshWorldSize;
            _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _meshWorldSize);

            UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
            var alreadyUpdatedChunkCoords = new HashSet<Vector2>();
            for (var i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].Coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
            var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

            for (var yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
            {
                for (var xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            var newChunk = new TerrainChunk(viewedChunkCoord, _heightMapSettings, _meshSettings,
                                _detailLevels, _colliderLODIndex, transform, _viewer, _mapMaterial, _loadAsync);
                            _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.MeshSet += OnTerrainChunkMeshSet;
                            newChunk.VisibilityChange += OnTerrainChunkVisibilityChange;
                            newChunk.Load();
                        }
                    }
                }
            }
        }

        private void OnTerrainChunkMeshSet(TerrainChunk terrainChunk)
        {
            terrainChunk.MeshSet -= OnTerrainChunkMeshSet;

            if (!_chunkGenerated)
            {
                _chunkGenerated = true;
                ChunkGenerated?.Invoke();
            }
        }

        private void OnTerrainChunkVisibilityChange(TerrainChunk chunk, bool isVisible)
        {
            if (isVisible)
            {
                _visibleTerrainChunks.Add(chunk);
            }
            else
            {
                _visibleTerrainChunks.Remove(chunk);
            }
        }
    }
}