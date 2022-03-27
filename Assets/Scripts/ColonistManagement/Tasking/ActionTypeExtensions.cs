using System;

namespace ColonistManagement.Tasking
{
    public static class ActionTypeExtensions
    {
        public static string GetString(this ActionType actionType)
        {
            return actionType switch {
                ActionType.FollowingOrders => "Following Orders",
                ActionType.Relaxing => "Relaxing",
                ActionType.CuttingWood => "Cutting - Wood",
                ActionType.MiningStone => "Mining - Stone",
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
            };
        }
    }
}
