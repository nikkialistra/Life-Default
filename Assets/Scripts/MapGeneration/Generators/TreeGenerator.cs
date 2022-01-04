using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration.Generators
{
    public class TreeGenerator : MonoBehaviour
    {
        [Title("Boundaries")]
        [MinValue(0)]
        [SerializeField] private float _xBounds;
        [MinValue(0)]
        [SerializeField] private float _zBounds;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeight;

        [Title("Parameters")]
        [Range(0, 1)]
        [SerializeField] private float _spawnProbability;
        [Range(0, 0.5f)]
        [SerializeField] private float _density;
        [SerializeField] private float _offset;
        [SerializeField] private int _seed;
        [Space]
        [Required]
        [SerializeField] private GameObject _treePrefab;

        private readonly List<GameObject> _trees = new();

        private LayerMask _terrainMask;

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
        }

        [Button(ButtonSizes.Large)]
        public void Generate()
        {
            RemoveTrees();
            Random.InitState(_seed);

            for (var z = -_zBounds; z < _zBounds; z += 10f)
            {
                for (var x = -_xBounds; x < _xBounds; x += 10f)
                {
                    TrySpawnTree(x, z);
                }
            }
        }

        private void TrySpawnTree(float x, float z)
        {
            if (!ShouldSpawn(x, z))
            {
                return;
            }

            CheckForBoundaries(x, z);
        }

        private bool ShouldSpawn(float x, float z)
        {
            var sample = Mathf.PerlinNoise((x * _density) + _offset, (z * _density) + _offset);
            var gauss = 8f * Random.Range(0f, 1f);

            return sample < _spawnProbability || gauss < _spawnProbability;
        }

        private void CheckForBoundaries(float x, float z)
        {
            var origin = new Vector3(x, 100f, z);

            if (Physics.Raycast(origin, Vector3.down, out var hit, Mathf.Infinity, _terrainMask))
            {
                Debug.Log(1);
                if (hit.point.y > _minHeight && hit.point.y < _maxHeight)
                {
                    SpawnTree(hit);
                }
            }
        }

        private void SpawnTree(RaycastHit hit)
        {
            var position = new Vector3(hit.point.x + Random.Range(-3.33f, 3.33f), hit.point.y - 0.5f,
                hit.point.z + Random.Range(-3.33f, 3.33f));

            var tree = Instantiate(_treePrefab, position, Quaternion.identity);

            _trees.Add(tree);
        }

        private void RemoveTrees()
        {
            foreach (var tree in _trees)
            {
                DestroyImmediate(tree);
            }

            _trees.Clear();
        }
    }
}