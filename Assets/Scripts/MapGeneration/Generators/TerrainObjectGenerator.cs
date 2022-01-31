using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration.Generators
{
    public class TerrainObjectGenerator : MonoBehaviour
    {
        [SerializeField] private bool _autoUpdate;

        [Title("Boundaries")]
        [MinValue(0)]
        [SerializeField] private float _xBounds;
        [MinValue(0)]
        [SerializeField] private float _zBounds;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeight;

        [Title("Parameters")]
        [Range(0, 1)]
        [SerializeField] private float _probability;
        [SerializeField] private int _probabilitySeed;
        [Space]
        [Range(0, 0.1f)]
        [SerializeField] private float _dispersion;
        [SerializeField] private float _dispersionOffset;
        [Space]
        [Range(5, 20f)]
        [SerializeField] private float _pointInterval;
        [Range(0, 10f)]
        [SerializeField] private float _pointVariation;
        [Range(0, 360f)]
        [SerializeField] private float _rotationVariation;
        [SerializeField] private float _heightSetCorrection;

        [Title("Objects")]
        [Required]
        [SerializeField] private List<TerrainObject> _prefabs;

        private readonly List<TerrainObject> _terrainObjects = new();

        private LayerMask _terrainMask;

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
        }

        private void OnValidate()
        {
            if (_autoUpdate)
            {
                Generate();
            }
        }

        [Button(ButtonSizes.Large)]
        public void Generate()
        {
            RemoveTerrainObjects();
            Random.InitState(_probabilitySeed);

            for (var z = -_zBounds; z < _zBounds; z += _pointInterval)
            {
                for (var x = -_xBounds; x < _xBounds; x += _pointInterval)
                {
                    TrySpawn(x, z);
                }
            }
        }

        private void TrySpawn(float x, float z)
        {
            if (!ShouldSpawn(x, z))
            {
                return;
            }

            CheckForBoundaries(x, z);
        }

        private bool ShouldSpawn(float x, float z)
        {
            var sample = Mathf.PerlinNoise((x * _dispersion) + _dispersionOffset,
                (z * _dispersion) + _dispersionOffset);
            var gauss = 8f * Random.Range(0f, 1f);

            return sample < _probability || gauss < _probability;
        }

        private void CheckForBoundaries(float x, float z)
        {
            var origin = new Vector3(x, 1000f, z);

            if (Physics.Raycast(origin, Vector3.down, out var hit, Mathf.Infinity, _terrainMask))
            {
                if (hit.point.y > _minHeight && hit.point.y < _maxHeight)
                {
                    Spawn(hit);
                }
            }
        }

        private void Spawn(RaycastHit hit)
        {
            var prefab = _prefabs[Random.Range(0, _prefabs.Count)];

            var position = new Vector3(hit.point.x + Random.Range(-_pointVariation, _pointVariation),
                hit.point.y + _heightSetCorrection,
                hit.point.z + Random.Range(-_pointVariation, _pointVariation));
            var rotation = Quaternion.Euler(0, Random.Range(0, _rotationVariation), 0);

            var terrainObject = Instantiate(prefab, position, Quaternion.identity, transform);
            terrainObject.Rotate(rotation);

            _terrainObjects.Add(terrainObject);
        }

        private void RemoveTerrainObjects()
        {
            foreach (var terrainObject in _terrainObjects)
            {
                Destroy(terrainObject);
            }

            _terrainObjects.Clear();
        }
    }
}
