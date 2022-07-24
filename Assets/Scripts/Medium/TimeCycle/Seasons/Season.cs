using System;

namespace Medium.TimeCycle.Seasons
{
    [Flags]
    public enum Season
    {
        Spring = 1 << 0,
        Summer = 1 << 1,
        Autumn = 1 << 2,
        Winter = 1 << 3
    }
}
