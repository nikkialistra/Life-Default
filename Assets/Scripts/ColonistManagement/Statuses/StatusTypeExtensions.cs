using System;

namespace ColonistManagement.Statuses
{
    public static class StatusTypeExtensions
    {
        public static string GetString(this StatusType statusType)
        {
            return statusType switch {
                StatusType.None => "",
                StatusType.Hungry => "Hungry",
                StatusType.NoEnoughSleep => "Don't have enough sleep",
                StatusType.Dizzy => "Dizzy",
                _ => throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null)
            };
        }
    }
}
