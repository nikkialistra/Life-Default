using System;

namespace Units.Enums
{
    public static class FactionExtensions
    {
        public static string GetStringForMultiple(this Faction faction)
        {
            return faction switch
            {
                Faction.Colonists => "colonists",
                Faction.Aborigines => "aborigines",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
