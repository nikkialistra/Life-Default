using System;
using System.Collections.Generic;
using System.Linq;
using Colonists.Skills;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists
{
    public class ColonistSkills : MonoBehaviour
    {
        [ValidateInput(nameof(ColonistHaveEachSkill))]
        [SerializeField] private SkillsDictionary _skills;

        public IReadOnlyList<Skill> Skills => _skills.Values.ToList(); 

        public void ImproveSkill(SkillType skillType, int quantity)
        {
            _skills[skillType].ImproveBy(quantity);
        }

        public Skill GetSkill(SkillType skillType)
        {
            return _skills[skillType];
        }

        private bool ColonistHaveEachSkill(SkillsDictionary skills, ref string errorMessage)
        {
            foreach (var skillType in (SkillType[])Enum.GetValues(typeof(SkillType)))
            {
                if (!skills.ContainsKey(skillType))
                {
                    errorMessage = $"Skills don't contain {skillType} skill";
                    return false;
                }

                if (skills[skillType].SkillType != skillType)
                {
                    errorMessage = $"Skill for {skillType} have inconsistent type {skills[skillType].SkillType}";
                    return false;
                }
            }

            return true;
        }
        
        [Serializable] public class SkillsDictionary : SerializableDictionary<SkillType, Skill> { }
    }
}
