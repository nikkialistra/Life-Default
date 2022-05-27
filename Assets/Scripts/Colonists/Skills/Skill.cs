using System;
using UnityEngine;

namespace Colonists.Skills
{
    [Serializable]
    public class Skill
    {
        [SerializeField] private SkillType _skillType;
        [SerializeField] private FavoriteLevel _favoriteLevel;
        
        [Space]
        [Range(0, MaxLevel)]
        [SerializeField] private int _level;
        [Range(0, ProgressRange)]
        [SerializeField] private int _progress;

        private const int MaxLevel = 9;
        private const int ProgressRange = 1000;

        public SkillType SkillType => _skillType;
        public FavoriteLevel FavoriteLevel => _favoriteLevel;

        public int Level => _level;
        public int Progress => _progress;

        public void ImproveBy(int quantity)
        {
            if (_level == MaxLevel)
            {
                return;
            }
            
            _progress += quantity;

            if (_progress >= ProgressRange)
            {
                _progress -= ProgressRange;
                _level++;
            }
        }
    }
}
