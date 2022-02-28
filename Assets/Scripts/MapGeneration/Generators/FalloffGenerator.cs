using MapGeneration.Data;
using UnityEngine;

namespace MapGeneration.Generators
{
    public static class FalloffGenerator
    {
        public static HeightMap GenerateFalloffMap(int resolution)
        {
            var values = GenerateFalloffValues(resolution);
            
            return new HeightMap(values, 0, 1);
        }

        public static float[,] GenerateFalloffValues(int resolution)
        {
            var values = new float[resolution, resolution];

            for (var i = 0; i < resolution; i++)
            {
                for (var j = 0; j < resolution; j++)
                {
                    var x = i / (float)resolution * 2 - 1;
                    var y = j / (float)resolution * 2 - 1;

                    var value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    values[i, j] = Evaluate(value);
                }
            }

            return values;
        }

        private static float Evaluate(float value)
        {
            float a = 3;
            var b = 2.2f;

            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }
}
