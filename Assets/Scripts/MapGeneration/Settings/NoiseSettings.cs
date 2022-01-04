using System;
using MapGeneration.Generators;
using UnityEngine;

namespace MapGeneration.Settings
{
    [Serializable]
    public class NoiseSettings
    {
        [SerializeField] private NoiseGenerator.NormalizeMode _normalizeMode;

        [SerializeField] private float _scale = 50;

        [SerializeField] private int _octaves = 6;
        [Range(0, 1)]
        [SerializeField] private float _persistence = .6f;
        [SerializeField] private float _lacunarity = 2;

        [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;

        public NoiseGenerator.NormalizeMode NormalizeMode => _normalizeMode;

        public float Scale => _scale;
        public int Octaves => _octaves;
        public float Persistence => _persistence;
        public float Lacunarity => _lacunarity;

        public int Seed => _seed;
        public Vector2 Offset => _offset;

        public void ValidateValues()
        {
            _scale = Mathf.Max(_scale, 0.01f);
            _octaves = Mathf.Max(_octaves, 1);
            _lacunarity = Mathf.Max(_lacunarity, 1);
            _persistence = Mathf.Clamp01(_persistence);
        }
    }
}