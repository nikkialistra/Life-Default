using System;

namespace Colonists.Skills
{
    public static class SkillTypeExtensions
    {
        public static string GetUxmlName(this SkillType skillType)
        {
            return skillType switch {
                SkillType.Strength => "strength",
                SkillType.Dexterity => "dexterity",
                SkillType.Accuracy => "accuracy",
                SkillType.Athletics => "athletics",
                SkillType.Construction => "construction",
                SkillType.Mining => "mining",
                SkillType.Research => "research",
                SkillType.Cooking => "cooking",
                SkillType.Communication => "communication",
                SkillType.Creativity => "creativity",
                SkillType.Medicine => "medicine",
                SkillType.Agriculture => "agriculture",
                SkillType.AnimalHandling => "animal-handling",
                
                _ => throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null)
            };
        }
    }
}
