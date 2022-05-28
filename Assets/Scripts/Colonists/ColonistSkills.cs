using System;
using System.Collections.Generic;
using System.Linq;
using Colonists.Activities;
using Colonists.Skills;
using Common;
using Infrastructure.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Colonists
{
    public class ColonistSkills : MonoBehaviour
    {
        [ValidateInput(nameof(ColonistHaveEachSkill))]
        [SerializeField] private SkillsDictionary _skills;

        public IReadOnlyList<Skill> Skills => _skills.Values.ToList();
        
        private float _skillProgressPerSecond;

        [Inject]
        public void Construct(SkillsSettings skillsSettings)
        {
            foreach (var skill in _skills.Values)
            {
                skill.Initialize(skillsSettings);
            }

            _skillProgressPerSecond = skillsSettings.SkillProgressPerSecond;
        }
        
        public void Advance(ActivityType activityType, float duration)
        {
            var skillType = activityType.ToSkillType();

            if (!skillType.HasValue)
            {
                return;
            }
            
            ImproveSkill(skillType.Value, duration * _skillProgressPerSecond);
        }

        public void ImproveSkill(SkillType skillType, float quantity)
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
