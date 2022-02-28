using MapGeneration.Settings;
using UnityEngine;
using Random = System.Random;

namespace MapGeneration.Generators
{
    public static class NoiseGenerator
    {
        public static float[,] GenerateNoiseMap(NoiseSettings settings,
            Vector2 sampleCenter)
        {
            var size = settings.Size;
            var resolution = settings.Resolution;

            var noiseMap = new float[resolution, resolution];

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

            var step = (float)size / resolution;

            for (var y = 0; y < resolution; y++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    var offsetX = x * step;
                    var offsetY = y * step;
                    
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    for (var i = 0; i < settings.Octaves; i++)
                    {
                        var sampleX = (offsetX - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                        var sampleY = (offsetY - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;

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

                    if (settings.GlobalMode)
                    {
                        var normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.8f);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            if (settings.GlobalMode)
            {
                return noiseMap;
            }

            for (var y = 0; y < resolution; y++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}
