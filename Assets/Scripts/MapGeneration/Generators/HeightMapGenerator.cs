using MapGeneration.Data;
using MapGeneration.Settings;
using UnityEngine;

namespace MapGeneration.Generators
{
    public static class HeightMapGenerator
    {
        public static HeightMap GenerateHeightMap(HeightMapSettings settings,
            Vector2 sampleCenter)
        {
            var values = NoiseGenerator.GenerateNoiseMap(settings.NoiseSettings, sampleCenter);

            var heightCurveThreadSafe = new AnimationCurve(settings.HeightCurve.keys);

            var size = settings.NoiseSettings.Size;

            var falloffMap = FalloffGenerator.GenerateFalloffMap(size);

            var minValue = float.MaxValue;
            var maxValue = float.MinValue;

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    if (settings.UseFalloff)
                    {
                        values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
                    }

                    values[i, j] = heightCurveThreadSafe.Evaluate(values[i, j]) * settings.HeightMultiplier;

                    if (values[i, j] > maxValue)
                    {
                        maxValue = values[i, j];
                    }

                    if (values[i, j] < minValue)
                    {
                        minValue = values[i, j];
                    }
                }
            }

            return new HeightMap(values, minValue, maxValue);
        }
    }
}
