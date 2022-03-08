using UnityEngine;

namespace Colonists.Colonist
{
    public static class ColonistNameGenerator
    {
        private static readonly string[] Names =
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
            return Names[Random.Range(0, Names.Length)];
        }
    }
}
