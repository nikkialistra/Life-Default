using System;
using UnityEngine;

namespace MapGeneration.Settings
{
    [CreateAssetMenu]
    public class HeightMapSettings : ScriptableObject
    {
        [SerializeField] private NoiseSettings _noiseSettings;

        [SerializeField] private float _heightMultiplier;
        [SerializeField] private AnimationCurve _heightCurve;

        public event Action Update;

        public NoiseSettings NoiseSettings => _noiseSettings;

        public float HeightMultiplier => _heightMultiplier;
        public AnimationCurve HeightCurve => _heightCurve;

        public float MinHeight => _heightMultiplier * _heightCurve.Evaluate(0);
        public float MaxHeight => _heightMultiplier * _heightCurve.Evaluate(1);
        
        protected void OnValidate()
        {
            Update?.Invoke();
        }
    }
}
