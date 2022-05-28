using System;
using Colonists.Skills;

namespace Colonists.Activities
{
    public static class ActivityTypeExtensions
    {
        public static SkillType? ToSkillType(this ActivityType activityType)
        {
            return activityType switch {
                ActivityType.Idle => null,
                ActivityType.Gathering => SkillType.Gathering,
                ActivityType.Fighting => SkillType.Accuracy,
                ActivityType.Hauling => SkillType.Athletics,
                ActivityType.Constructing => SkillType.Construction,
                
                _ => throw new ArgumentOutOfRangeException(nameof(activityType), activityType, null)
            };
        }
    }
}
