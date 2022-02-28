using MapGeneration.Data;
using MapGeneration.Settings;
using UnityEngine;

namespace MapGeneration.Generators
{
    public static class HeightMapGenerator
    {
        public static HeightMap GenerateNoiseMap(HeightMapSettings settings,
            Vector2 sampleCenter)
        {
            var values  = NoiseGenerator.GenerateNoiseMap(settings.NoiseSettings, sampleCenter);
            var noiseMap = PassThroughCurve(values, settings);
            return noiseMap;
        }
        
        public static HeightMap GenerateHeightMap(HeightMapSettings settings,
            Vector2 sampleCenter)
        {
            var values  = NoiseGenerator.GenerateNoiseMap(settings.NoiseSettings, sampleCenter);
            var valuesWithFalloff = PassThroughFalloffMap(values, settings);
            var heightMap = PassThroughCurve(valuesWithFalloff, settings);
            return heightMap;
        }

        private static float[,] PassThroughFalloffMap(float[,] values, HeightMapSettings settings)
        {
            var size = settings.NoiseSettings.Size;
            var falloffMap = FalloffGenerator.GenerateFalloffValues(size);

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
                }
            }

            return values;
        }

        private static HeightMap PassThroughCurve(float[,] values, HeightMapSettings settings)
        {
            var size = settings.NoiseSettings.Size;
            var heightCurve = new AnimationCurve(settings.HeightCurve.keys);
            
            var minValue = float.MaxValue;
            var maxValue = float.MinValue;

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    values[i, j] = heightCurve.Evaluate(values[i, j]) * settings.HeightMultiplier;

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
