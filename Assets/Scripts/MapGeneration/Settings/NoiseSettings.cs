using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration.Settings
{
    [Serializable]
    public class NoiseSettings
    {
        [SerializeField] private Resolutions _resolution;

        [SerializeField] private bool _globalMode;

        [MinValue(0.1f)]
        [SerializeField] private int _size;
        
        [SerializeField] private float _scale = 50;

        [SerializeField] private int _octaves = 6;
        [Range(0, 1)]
        [SerializeField] private float _persistence = .6f;
        [SerializeField] private float _lacunarity = 2;

        [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;

        private enum Resolutions
        {
            _257,
            _513,
            _1025,
            _2049,
            _4097
        }

        public int Resolution
        {
            get
            {
                return _resolution switch
                {
                    Resolutions._257 => 257,
                    Resolutions._513 => 513,
                    Resolutions._1025 => 1025,
                    Resolutions._2049 => 2049,
                    Resolutions._4097 => 4097,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public bool GlobalMode => _globalMode;

        public int Size => _size;
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
