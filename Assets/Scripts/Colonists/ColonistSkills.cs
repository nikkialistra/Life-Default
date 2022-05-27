using System;
using Colonists.Skills;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists
{
    public class ColonistSkills : MonoBehaviour
    {
        [ValidateInput(nameof(ColonistHaveEachSkill))]
        [SerializeField] private SkillsDictionary _skillses;

        public void ImproveSkill(SkillType skillType, int quantity)
        {
            _skillses[skillType].ImproveBy(quantity);
        }

        public Skill GetSkill(SkillType skillType)
        {
            return _skillses[skillType];
        }

        private bool ColonistHaveEachSkill(SkillsDictionary skillses, ref string errorMessage)
        {
            foreach (var skillType in (SkillType[])Enum.GetValues(typeof(SkillType)))
            {
                if (!skillses.ContainsKey(skillType))
                {
                    errorMessage = $"Skills don't contain {skillType} skill";
                    return false;
                }

                if (skillses[skillType].SkillType != skillType)
                {
                    errorMessage = $"Skill for {skillType} have inconsistent type {skillses[skillType].SkillType}";
                    return false;
                }
            }

            return true;
        }
        
        [Serializable] public class SkillsDictionary : SerializableDictionary<SkillType, Skill> { }
    }
}
