using System;
using UnityEngine;

namespace MapGeneration.Settings
{
    [CreateAssetMenu]
    public class HeightMapSettings : ScriptableObject
    {
        [SerializeField] private NoiseSettings _noiseSettings;
        [SerializeField] private AnimationCurve _heightCurve;

        public event Action Update;

        public NoiseSettings NoiseSettings => _noiseSettings;
        public AnimationCurve HeightCurve => _heightCurve;

        protected void OnValidate()
        {
            Update?.Invoke();
        }
    }
}
