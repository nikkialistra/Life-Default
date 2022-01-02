using System;

namespace Common
{
    public class EnumUtils
    {
        public static T RandomEnumValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            var random = UnityEngine.Random.Range(0, values.Length);
            return (T)values.GetValue(random);
        }
    }
}