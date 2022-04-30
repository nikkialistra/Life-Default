using System;

namespace Units.Enums
{
    public static class FractionExtensions
    {
        public static string GetStringForMultiple(this Fraction fraction)
        {
            return fraction switch
            {
                Fraction.Colonists => "colonists",
                Fraction.Enemies => "enemies",
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }
    }
}
