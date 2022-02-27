using MapGeneration.Settings;
using UnityEngine;
using Random = System.Random;

namespace MapGeneration.Generators
{
    public static class NoiseGenerator
    {
        public enum NormalizeMode
        {
            Local,
            Global
        };

        public static float[,] GenerateNoiseMap(NoiseSettings settings,
            Vector2 sampleCenter)
        {
            var size = settings.Size;

            var noiseMap = new float[size, size];

            var prng = new Random(settings.Seed);
            var octaveOffsets = new Vector2[settings.Octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (var i = 0; i < settings.Octaves; i++)
            {
                var offsetX = prng.Next(-100000, 100000) + settings.Offset.x + sampleCenter.x;
                var offsetY = prng.Next(-100000, 100000) - settings.Offset.y - sampleCenter.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= settings.Persistence;
            }

            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;

            var halfWidth = size / 2f;
            var halfHeight = size / 2f;


            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    for (var i = 0; i < settings.Octaves; i++)
                    {
                        var sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                        var sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.Persistence;
                        frequency *= settings.Lacunarity;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }

                    if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;

                    if (settings.NormalizeMode == NormalizeMode.Global)
                    {
                        var normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            if (settings.NormalizeMode != NormalizeMode.Local)
            {
                return noiseMap;
            }

            {
                for (var y = 0; y < size; y++)
                {
                    for (var x = 0; x < size; x++)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                }
            }

            return noiseMap;
        }
    }
}
