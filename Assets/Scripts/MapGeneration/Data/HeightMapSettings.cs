using UnityEngine;

namespace MapGeneration.Data
{
    [CreateAssetMenu]
    public class HeightMapSettings : UpdatableData
    {
        [SerializeField] private NoiseSettings _noiseSettings;

        [SerializeField] private bool _useFalloff;

        [SerializeField] private float _heightMultiplier;
        [SerializeField] private AnimationCurve _heightCurve;

        public NoiseSettings NoiseSettings => _noiseSettings;
        public float HeightMultiplier => _heightMultiplier;
        public AnimationCurve HeightCurve => _heightCurve;

        public float MinHeight => _heightMultiplier * _heightCurve.Evaluate(0);
        public float MaxHeight => _heightMultiplier * _heightCurve.Evaluate(1);

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            _noiseSettings.ValidateValues();
            base.OnValidate();
        }
#endif
    }
}