using System;

namespace ColonistManagement.Statuses
{
    public static class StatusTypeExtensions
    {
        public static string GetString(this StatusType statusType)
        {
            return statusType switch {
                StatusType.None => "",
                StatusType.Injured => "Injured",
                StatusType.Hungry => "Hungry",
                StatusType.NoEnoughSleep => "Don't have enough sleep",
                StatusType.Unhappy => "Unhappy",
                StatusType.Dizzy => "Dizzy",
                StatusType.Bored => "Bored",
                _ => throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null)
            };
        }
    }
}
