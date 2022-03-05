using MapGeneration.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration.Generators
{
    public class HeightMapTransformer : MonoBehaviour
    {
        [SerializeField] private Texture2D _noise;
        [SerializeField] private AnimationCurve _heightCurve;

        [Button(ButtonSizes.Medium)]
        public void ConvertTexture(string name = "HeightMapTest.png")
        {
            if (_noise.width != _noise.height)
            {
                Debug.LogError("The noise texture doesn't have square form");
                return;
            }

            var resolution = _noise.width;
            var values = GetValues(resolution);

            var valuesWithFalloff = PassThroughFalloffMap(values, resolution);
            var heightMap = PassThroughCurve(valuesWithFalloff, resolution);

            TextureGenerator.ExportHeightMap(heightMap, name);
        }

        private float[,] GetValues(int resolution)
        {
            var values = new float[resolution, resolution];
            var colors = _noise.GetPixels();

            for (var i = 0; i < resolution; i++)
            {
                for (var j = 0; j < resolution; j++)
                {
                    values[i, j] = colors[i * resolution + j].r;
                }
            }

            return values;
        }

        private float[,] PassThroughFalloffMap(float[,] values, int resolution)
        {
            var falloffMap = FalloffGenerator.GenerateFalloffValues(resolution);

            for (var i = 0; i < resolution; i++)
            {
                for (var j = 0; j < resolution; j++)
                {
                    values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
                }
            }

            return values;
        }
        
        private HeightMap PassThroughCurve(float[,] values, int resolution)
        {
            var minValue = float.MaxValue;
            var maxValue = float.MinValue;

            for (var i = 0; i < resolution; i++)
            {
                for (var j = 0; j < resolution; j++)
                {
                    values[i, j] = _heightCurve.Evaluate(values[i, j]);

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
