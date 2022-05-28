using System;
using Infrastructure.Settings;
using UnityEngine;

namespace Colonists.Skills
{
    [Serializable]
    public class Skill
    {
        [SerializeField] private SkillType _skillType;
        [SerializeField] private bool _canDo = true;
        
        [Space]
        [SerializeField] private FavoriteLevel _favoriteLevel;

        [Range(0, MaxLevel)]
        [SerializeField] private int _level;
        [Range(0, ProgressRange)]
        [SerializeField] private float _progress;

        private const int MaxLevel = 9;
        private const int ProgressRange = 1000;
        
        private float _oneStarFavoriteMultiplier;
        private float _twoStarsFavoriteMultiplier;
        
        public void Initialize(SkillsSettings skillsSettings)
        {
            _oneStarFavoriteMultiplier = skillsSettings.OneStarFavoriteMultiplier;
            _twoStarsFavoriteMultiplier = skillsSettings.TwoStarsFavoriteMultiplier;
        }

        public SkillType SkillType => _skillType;
        public bool CanDo => _canDo;
        
        public FavoriteLevel FavoriteLevel => _favoriteLevel;

        public int Level => _level;
        public int Progress => (int)_progress;

        public void ImproveBy(float quantity)
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

        private int EnlargeWhenFavorite(int quantity)
        {
            return _favoriteLevel switch {
                FavoriteLevel.None => quantity,
                FavoriteLevel.OneStar => (int)(quantity * _oneStarFavoriteMultiplier),
                FavoriteLevel.TwoStars => (int)(quantity * _twoStarsFavoriteMultiplier),
                
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
