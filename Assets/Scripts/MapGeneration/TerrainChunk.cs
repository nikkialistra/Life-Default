using MapGeneration.Data;
using UnityEngine;

namespace MapGeneration
{
    public class TerrainChunk
    {
        private const float ColliderGenerationDistanceThreshold = 5;

        private Vector2 _coord;

        private readonly GameObject _meshObject;
        private readonly Vector2 _sampleCenter;
        private Bounds _bounds;

        private readonly MeshRenderer _meshRenderer;
        private readonly MeshFilter _meshFilter;
        private readonly MeshCollider _meshCollider;

        private readonly LODInfo[] _detailLevels;
        private readonly LODMesh[] _lodMeshes;
        private readonly int _colliderLODIndex;

        private HeightMap _heightMap;
        private bool _heightMapReceived;
        private int _previousLODIndex = -1;
        private bool _hasSetCollider;
        private readonly float _maxViewDst;

        private readonly HeightMapSettings _heightMapSettings;
        private readonly MeshSettings _meshSettings;
        private readonly Transform _viewer;

        public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings,
            LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
        {
            _coord = coord;
            _detailLevels = detailLevels;
            _colliderLODIndex = colliderLODIndex;
            _heightMapSettings = heightMapSettings;
            _meshSettings = meshSettings;
            _viewer = viewer;

            _sampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
            var position = coord * meshSettings.MeshWorldSize;
            _bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);


            _meshObject = new GameObject("Terrain Chunk");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            _meshRenderer.material = material;

            _meshObject.transform.position = new Vector3(position.x, 0, position.y);
            _meshObject.transform.parent = parent;
            SetVisible(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (var i = 0; i < detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(detailLevels[i].Lod);
                _lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
                if (i == colliderLODIndex)
                {
                    _lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
                }
            }

            _maxViewDst = detailLevels[detailLevels.Length - 1].VisibleDistanceThreshold;
        }

        public event System.Action<TerrainChunk, bool> VisibilityChange;

        public Vector2 Coord => _coord;

        public void Load()
        {
            ThreadedDataRequester.RequestData(
                () => HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine,
                    _heightMapSettings, _sampleCenter), OnHeightMapReceived);
        }

        public void UpdateCollisionMesh()
        {
            if (_hasSetCollider)
            {
                return;
            }

            var sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

            if (sqrDstFromViewerToEdge < _detailLevels[_colliderLODIndex].SqrVisibleDstThreshold)
            {
                if (!_lodMeshes[_colliderLODIndex].HasRequestedMesh)
                {
                    _lodMeshes[_colliderLODIndex].RequestMesh(_heightMap, _meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold)
            {
                if (_lodMeshes[_colliderLODIndex].HasMesh)
                {
                    _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].Mesh;
                    _hasSetCollider = true;
                }
            }
        }

        public void UpdateTerrainChunk()
        {
            if (!_heightMapReceived)
            {
                return;
            }

            var viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

            var wasVisible = IsVisible();
            var visible = viewerDstFromNearestEdge <= _maxViewDst;

            if (visible)
            {
                var lodIndex = 0;

                for (var i = 0; i < _detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > _detailLevels[i].VisibleDistanceThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != _previousLODIndex)
                {
                    var lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.HasMesh)
                    {
                        _previousLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.Mesh;
                    }
                    else if (!lodMesh.HasRequestedMesh)
                    {
                        lodMesh.RequestMesh(_heightMap, _meshSettings);
                    }
                }
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);
                VisibilityChange?.Invoke(this, visible);
            }
        }

        private void OnHeightMapReceived(object heightMapObject)
        {
            _heightMap = (HeightMap)heightMapObject;
            _heightMapReceived = true;

            UpdateTerrainChunk();
        }

        private Vector2 ViewerPosition => new(_viewer.position.x, _viewer.position.z);

        private void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        private bool IsVisible()
        {
            return _meshObject.activeSelf;
        }

        private class LODMesh
        {
            public Mesh Mesh;
            public bool HasRequestedMesh;
            public bool HasMesh;

            private readonly int _lod;

            public LODMesh(int lod)
            {
                _lod = lod;
            }

            public event System.Action UpdateCallback;

            void OnMeshDataReceived(object meshDataObject)
            {
                Mesh = ((MeshData)meshDataObject).CreateMesh();
                HasMesh = true;

                UpdateCallback?.Invoke();
            }

            public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
            {
                HasRequestedMesh = true;
                ThreadedDataRequester.RequestData(
                    () => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, _lod),
                    OnMeshDataReceived);
            }
        }
    }
}