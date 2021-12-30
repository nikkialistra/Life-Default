using UnityEngine;

namespace Units.Unit
{
    public class UnitNameGenerator
    {
        private static string[] _names = new[]
        {
            "Tiffany",
            "James",
            "Mary",
            "Patrick",
            "Sandra",
            "Kyle",
            "Sean",
            "Dylan",
            "Juan",
            "Roy",
            "Elijah"
        };

        public static string GetRandomName()
        {
            return _names[Random.Range(0, _names.Length)];
        }
    }
}