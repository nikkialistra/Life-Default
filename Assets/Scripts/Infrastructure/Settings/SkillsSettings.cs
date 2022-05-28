using System;
using UnityEngine;

namespace Infrastructure.Settings
{
    [Serializable]
    public class SkillsSettings
    {
        // 1000 points per 180 seconds (1000 / 180)
        public float SkillProgressPerSecond = 5.55f;

        [Space]
        public float OneStarFavoriteMultiplier = 1.5f;
        public float TwoStarsFavoriteMultiplier = 2f;
    }
}
