using System.Collections;
using System.Collections.Generic;
using Enemies;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Units.Services
{
    public class UnitGenerator : MonoBehaviour
    {
        [SerializeField] private bool _spawnAtStart = true;

        [Title("Boundaries")]
        [MinValue(0)]
        [SerializeField] private float _xBounds;
        [MinValue(0)]
        [SerializeField] private float _zBounds;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeight;
        
        [Title("Intervals")]
        [SerializeField] private float _timeBetweenSpawns = 16f;
        [SerializeField] private float _timeBetweenSpawnsVariation = 4f;
        [MinValue(1)]
        [SerializeField] private int _maxInstancesPerSpawn;

        [Title("Parameters")]
        [SerializeField] private int _probabilitySeed;
        [Space]
        [Range(0, 1f)]
        [SerializeField] private float _probability;
        [Space]
        [MinValue(1f)]
        [SerializeField] private float _scale;
        [SerializeField] private float _xOffset;
        [SerializeField] private float _zOffset;
        
        [Title("Position Variation")]
        [Range(0, 10f)]
        [SerializeField] private float _pointVariation;
        [Range(0, 360f)]
        [SerializeField] private float _rotationVariation;
        
        [Title("Testing")]
        [SerializeField] private float _gridInterval = 3f;
        [SerializeField] private Transform _cubesParent;
        [SerializeField] private float _cubeScale = 0.3f;

        private LayerMask _terrainMask;
        
        private Enemy.Factory _enemyFactory;

        private bool _generateTestCubes;
        private readonly List<GameObject> _cubes = new();

        private Coroutine _generateAtIntervalCoroutine;

        [Inject]
        public void Construct(Enemy.Factory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
        }

        private void Start()
        {
            if (_spawnAtStart)
            {
                StartSpawning();
            }
        }

        [Button(ButtonSizes.Medium)]
        private void StartSpawning()
        {
            if (_generateAtIntervalCoroutine != null)
            {
                return;
            }

            _generateAtIntervalCoroutine = StartCoroutine(GenerateAtInterval());
        }

        [Button(ButtonSizes.Medium)]
        private void ShowSpawnGrid()
        {
            HideSpawnGrid();
            
            _generateTestCubes = true;
            
            for (var z = -_zBounds; z < _zBounds; z += _gridInterval)
            {
                for (var x = -_xBounds; x < _xBounds; x += _gridInterval)
                {
                    TrySpawn(x, z);
                }
            }

            _generateTestCubes = false;
        }

        [Button(ButtonSizes.Medium)]
        private void HideSpawnGrid()
        {
            foreach (var cube in _cubes)
            {
                Destroy(cube);
            }
        }

        private IEnumerator GenerateAtInterval()
        {
            while (true)
            {
                yield return new WaitForSeconds(CalculateInterval());
                
                Generate();
            }
        }

        private float CalculateInterval()
        {
            return _timeBetweenSpawns + Random.Range(-_timeBetweenSpawnsVariation, _timeBetweenSpawnsVariation);
        }

        private void Generate()
        {
            var x = Random.Range(-_xBounds, _xBounds);
            var z = Random.Range(-_zBounds, _zBounds);
            
            TrySpawn(x, z);
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
            var sample = Mathf.PerlinNoise((x * ( 1 / _scale) ) + _xOffset,
                (z * ( 1 / _scale) ) + _zOffset);
            
            Random.InitState(_probabilitySeed);
            var gauss = 8f * Random.Range(0f, 1f);

            return sample < _probability && gauss < _probability;
        }

        private void CheckForBoundaries(float x, float z)
        {
            var origin = new Vector3(x, 100f, z);

            if (Physics.Raycast(origin, Vector3.down, out var hit, Mathf.Infinity, _terrainMask))
            {
                if (hit.point.y > _minHeight && hit.point.y < _maxHeight)
                {
                    SpawnInstances(hit);
                }
            }
        }

        private void SpawnInstances(RaycastHit hit)
        {
            var count = Random.Range(1, _maxInstancesPerSpawn + 1);

            for (int i = 0; i < count; i++)
            {
                Spawn(hit);
            }
        }

        private void Spawn(RaycastHit hit)
        {
            var position = new Vector3(hit.point.x + Random.Range(-_pointVariation, _pointVariation), hit.point.y,
                hit.point.z + Random.Range(-_pointVariation, _pointVariation));
            var rotation = Quaternion.Euler(0, Random.Range(0, _rotationVariation), 0);

            PlaceAt(position, rotation);
            
        }

        private void PlaceAt(Vector3 position, Quaternion rotation)
        {
            if (!_generateTestCubes)
            {
                var enemy = _enemyFactory.Create();
                enemy.SetAt(position, rotation);
            }
            else
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                
                cube.transform.parent = _cubesParent;
                cube.transform.localScale *= _cubeScale;
                cube.transform.position = position;
                
                _cubes.Add(cube);
            }
        }
    }
}