using System;
using System.Collections.Generic;
using MapGeneration.Saving;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace MapGeneration.Generators
{
    [RequireComponent(typeof(NavMeshGenerator))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private bool _autoSave;

        [Title("Generators")]
        [Required]
        [SerializeField] private TerrainGenerator _terrainGenerator;
        [SerializeField] private List<TerrainObjectGenerator> _terrainObjectGenerators;

        private NavMeshGenerator _navMeshGenerator;

        private bool _generated;

        private void Awake()
        {
            _navMeshGenerator = GetComponent<NavMeshGenerator>();
        }

        public event Action Load;

        private void Start()
        {
            if (!_generated)
            {
                _terrainGenerator.Generate();
                _generated = true;
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

            var gameObjectContext = GetComponent<GameObjectContext>();
            var defaultGameObjectKernel = GetComponent<DefaultGameObjectKernel>();

            DestroyImmediate(gameObjectContext);
            DestroyImmediate(defaultGameObjectKernel);

            MapSaving.SavePrefab(gameObject, "Map.prefab");
        }

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void DeleteMap()
        {
            MapSaving.ClearSavedAssets();
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
                var savedPrefab = MapSaving.GetSavedPrefab("Map");
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

        public class Factory : PlaceholderFactory<MapGenerator>
        {
        }
    }
}