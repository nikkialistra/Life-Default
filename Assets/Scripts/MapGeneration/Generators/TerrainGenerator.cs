using System.Collections.Generic;
using MapGeneration.Data;
using MapGeneration.Settings;
using UnityEngine;

namespace MapGeneration.Generators
{
    [RequireComponent(typeof(TreeGenerator))]
    public class TerrainGenerator : MonoBehaviour
    {
        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float SqrViewerMoveThresholdForChunkUpdate =
            ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

        [SerializeField] private int _colliderLODIndex;
        [SerializeField] private LODInfo[] _detailLevels;

        [SerializeField] private MeshSettings _meshSettings;
        [SerializeField] private HeightMapSettings _heightMapSettings;

        [SerializeField] private Transform _viewer;
        [SerializeField] private Material _mapMaterial;

        private Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;

        private float _meshWorldSize;
        private int _chunksVisibleInViewDst;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new();
        private readonly List<TerrainChunk> _visibleTerrainChunks = new();

        private TreeGenerator _treeGenerator;

        private void Awake()
        {
            _treeGenerator = GetComponent<TreeGenerator>();
        }

        private void Start()
        {
            var maxViewDst = _detailLevels[^1].VisibleDistanceThreshold;
            _meshWorldSize = _meshSettings.MeshWorldSize;
            _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _meshWorldSize);

            UpdateVisibleChunks();
        }

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
                                _detailLevels, _colliderLODIndex, transform, _viewer, _mapMaterial);
                            _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.MeshSet += OnTerrainChunkMeshSet;
                            newChunk.VisibilityChange += OnTerrainChunkVisibilityChange;
                            newChunk.Load();
                        }
                    }
                }
            }
        }

        private void OnTerrainChunkMeshSet()
        {
            _treeGenerator.Generate();
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