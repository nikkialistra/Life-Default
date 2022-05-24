using System;

namespace Infrastructure.Settings
{
    [Serializable]
    public class AttackSettings
    {
        public float AttackRangeMultiplierToStartFight = 0.75f;
        public float AttackAngle = 60f;
        public float SeekPredictionMultiplier = 2f;
    }
}
