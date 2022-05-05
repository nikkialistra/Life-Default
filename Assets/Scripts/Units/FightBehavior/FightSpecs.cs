using System;

namespace Units.FightBehavior
{
    public readonly struct FightSpecs
    {
        public FightSpecs(float health, float averageDamagePerSecond)
        {
            if (health <= 0 || averageDamagePerSecond <= 0)
            {
                throw new ArgumentException("Unit has invalid specs");
            }
            
            Health = health;
            AverageDamagePerSecond = averageDamagePerSecond;
        }

        private float Health { get; }
        private float AverageDamagePerSecond { get; }

        public float WouldWinInTime(FightSpecs opponent, float advanceTime)
        {
            if (opponent.Health - (AverageDamagePerSecond * advanceTime) > 0)
            {
                return float.NegativeInfinity;
            }

            return opponent.Health / AverageDamagePerSecond;
        }

        public float WouldLoseInTime(FightSpecs opponent, float advanceTime)
        {
            if (Health - (opponent.AverageDamagePerSecond * advanceTime) > 0)
            {
                return float.NegativeInfinity;
            }

            return Health / opponent.AverageDamagePerSecond;
        }
    }
}
