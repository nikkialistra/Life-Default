using MapGeneration.Data;
using MapGeneration.Settings;
using UnityEngine;

namespace MapGeneration.Generators
{
    public static class HeightMapGenerator
    {
        public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings,
            Vector2 sampleCenter)
        {
            var values = NoiseGenerator.GenerateNoiseMap(width, height, settings.NoiseSettings, sampleCenter);

            var heightCurveThreadSafe = new AnimationCurve(settings.HeightCurve.keys);

            var minValue = float.MaxValue;
            var maxValue = float.MinValue;

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    values[i, j] *= heightCurveThreadSafe.Evaluate(values[i, j]) * settings.HeightMultiplier;

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