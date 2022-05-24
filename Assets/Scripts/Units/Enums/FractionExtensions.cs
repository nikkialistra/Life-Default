using System;

namespace Units.Enums
{
    public static class FractionExtensions
    {
        public static string GetStringForMultiple(this Faction faction)
        {
            return faction switch
            {
                Faction.Colonists => "colonists",
                Faction.Enemies => "enemies",
                _ => throw new ArgumentOutOfRangeException(nameof(faction), faction, null)
            };
        }
    }
}
