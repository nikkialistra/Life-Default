using System.Collections;
using System.Collections.Generic;
using Enemies;
using Enemies.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Units.Services
{
    public class UnitGenerator : MonoBehaviour
    {
        [SerializeField] private bool _spawnAtStart = true;
        [Space]
        [SerializeField] private int _maxEnemyCount = 20;

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
        [SerializeField] private int _numberOfTriesToSpawn = 5;

        [Title("Parameters")]
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

        [Title("Casting")]
        [SerializeField] private LayerMask _terrainMask;
        [SerializeField] private LayerMask _obstacleMask;
        [Space]
        [SerializeField] private float _obstacleRadius = 5f;

        private Enemy.Factory _enemyFactory;

        private EnemyRepository _enemyRepository;

        private bool _generateTestCubes;
        private readonly List<GameObject> _cubes = new();

        private Coroutine _generateAtIntervalCoroutine;

        private readonly Collider[] _obstacleResults = new Collider[1];

        [Inject]
        public void Construct(Enemy.Factory enemyFactory, EnemyRepository enemyRepository)
        {
            _enemyFactory = enemyFactory;
            _enemyRepository = enemyRepository;
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
            if (_generateAtIntervalCoroutine != null) return;

            _generateAtIntervalCoroutine = StartCoroutine(CGenerateAtInterval());
        }

        [Button(ButtonSizes.Medium)]
        private void ShowSpawnGrid()
        {
            HideSpawnGrid();

            _generateTestCubes = true;

            for (float z = -_zBounds; z < _zBounds; z += _gridInterval)
                for (float x = -_xBounds; x < _xBounds; x += _gridInterval)
                    Spawn(x, z);

            _generateTestCubes = false;
        }

        [Button(ButtonSizes.Medium)]
        private void HideSpawnGrid()
        {
            foreach (var cube in _cubes)
                Destroy(cube);
        }

        private IEnumerator CGenerateAtInterval()
        {
            while (true)
            {
                yield return new WaitForSeconds(CalculateInterval());

                if (_enemyRepository.Count <= _maxEnemyCount)
                    Generate();
            }
        }

        private float CalculateInterval()
        {
            return _timeBetweenSpawns + Random.Range(-_timeBetweenSpawnsVariation, _timeBetweenSpawnsVariation);
        }

        private void Generate()
        {
            var numberOfTries = 0;

            while (numberOfTries < _numberOfTriesToSpawn)
            {
                if (TrySpawn()) break;

                numberOfTries++;
            }
        }

        private bool TrySpawn()
        {
            var x = Random.Range(-_xBounds, _xBounds);
            var z = Random.Range(-_zBounds, _zBounds);

            if (!ShouldSpawn(x, z) || !InValidPlace(x, z, out var spawnPoint))
                return false;

            SpawnInstances(spawnPoint);

            return true;
        }

        private void Spawn(float x, float z)
        {
            if (!ShouldSpawn(x, z) || !InValidPlace(x, z, out var spawnPoint)) return;

            SpawnInstances(spawnPoint);
        }

        private bool ShouldSpawn(float x, float z)
        {
            var sample = Mathf.PerlinNoise((x * (1 / _scale)) + _xOffset,
                (z * (1 / _scale)) + _zOffset);

            return sample < _probability;
        }

        private bool InValidPlace(float x, float z, out Vector3 hitPoint)
        {
            var origin = new Vector3(x, 100f, z);

            if (Physics.Raycast(origin, Vector3.down, out var hit, Mathf.Infinity, _terrainMask))
            {
                if (IsValidPoint(hit.point))
                {
                    hitPoint = hit.point;
                    return true;
                }
            }

            hitPoint = default;
            return false;
        }

        private bool IsValidPoint(Vector3 point)
        {
            if (point.y < _minHeight || point.y > _maxHeight)
                return false;

            var size = Physics.OverlapSphereNonAlloc(point, _obstacleRadius, _obstacleResults, _obstacleMask);

            return size == 0;
        }

        private void SpawnInstances(Vector3 spawnPoint)
        {
            var count = Random.Range(1, _maxInstancesPerSpawn + 1);

            for (int i = 0; i < count; i++)
                Spawn(spawnPoint);
        }

        private void Spawn(Vector3 spawnPoint)
        {
            var position = new Vector3(spawnPoint.x + Random.Range(-_pointVariation, _pointVariation), spawnPoint.y,
                spawnPoint.z + Random.Range(-_pointVariation, _pointVariation));
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
