using System;
using Random = UnityEngine.Random;

namespace Common
{
    public class EnumUtils
    {
        public static T RandomValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            var random = Random.Range(0, values.Length);
            return (T)values.GetValue(random);
        }
    }
}
