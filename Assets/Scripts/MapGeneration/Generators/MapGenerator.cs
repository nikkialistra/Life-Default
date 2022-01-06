using System;
using System.Collections.Generic;
using System.IO;
using Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace MapGeneration.Generators
{
    [RequireComponent(typeof(NavMeshGenerator))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private bool _tryLoadFromSaved;
        [SerializeField] private bool _autoSave;

        [Title("Generators")]
        [Required]
        [SerializeField] private TerrainGenerator _terrainGenerator;
        [SerializeField] private List<TerrainObjectGenerator> _terrainObjectGenerators;

        private NavMeshGenerator _navMeshGenerator;

        private void Awake()
        {
            _navMeshGenerator = GetComponent<NavMeshGenerator>();
        }

        public event Action Load;

        private void Start()
        {
            if (!_tryLoadFromSaved || !TryLoad())
            {
                _terrainGenerator.Generate();
            }
        }

        private void OnEnable()
        {
            _terrainGenerator.ChunkGenerated += OnChunkGenerated;
        }

        private void OnDisable()
        {
            _terrainGenerator.ChunkGenerated -= OnChunkGenerated;
        }

#if UNITY_EDITOR

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void SaveMap()
        {
            foreach (var terrainChunk in _terrainGenerator.TerrainChunks)
            {
                terrainChunk.SaveMesh();
            }

            SaveUtils.CreateBaseDirectoriesTo(SaveUtils.SavedAssetsPath);

            var path = Path.Combine(SaveUtils.SavedAssetsPath, $"{gameObject.name}.prefab");
            PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        }

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void DeleteMap()
        {
            if (Directory.Exists(SaveUtils.SavedAssetsPath))
            {
                Directory.Delete(SaveUtils.SavedAssetsPath, true);
            }

            Directory.CreateDirectory(SaveUtils.SavedAssetsPath);
        }

#endif

        private void OnChunkGenerated()
        {
            GenerateTerrainObjects();
            GenerateNavMesh();

            Load?.Invoke();

#if UNITY_EDITOR

            if (_autoSave)
            {
                var savedPrefab = GetSavedPrefab();
                if (savedPrefab == null)
                {
                    SaveMap();
                }
            }

#endif
        }

        private void GenerateTerrainObjects()
        {
            foreach (var terrainObjectGenerator in _terrainObjectGenerators)
            {
                terrainObjectGenerator.Generate();
            }
        }

        private void GenerateNavMesh()
        {
            _navMeshGenerator.Build();
        }

        private bool TryLoad()
        {
            var savedPrefab = GetSavedPrefab();

            if (savedPrefab != null)
            {
                ReplaceSelf(savedPrefab);
                return true;
            }

            return false;
        }

        private GameObject GetSavedPrefab()
        {
            var path = Path.Combine(SaveUtils.SavedResourcesPath, "Map.prefab");
            var prefab = Resources.Load<GameObject>(path);
            return prefab;
        }

        private void ReplaceSelf(GameObject prefab)
        {
            Instantiate(prefab, transform.parent);
            Destroy(gameObject);
        }

        public class Factory : PlaceholderFactory<MapGenerator>
        {
        }
    }
}