using MapGeneration.Data;
using UnityEngine;

namespace MapGeneration.Generators
{
    public static class FalloffGenerator
    {
        public static HeightMap GenerateFalloffMap(int size)
        {
            var values = GenerateFalloffValues(size);
            
            return new HeightMap(values, 0, 1);
        }

        public static float[,] GenerateFalloffValues(int size)
        {
            var values = new float[size, size];

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var x = i / (float)size * 2 - 1;
                    var y = j / (float)size * 2 - 1;

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
