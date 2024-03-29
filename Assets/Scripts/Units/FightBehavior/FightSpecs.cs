﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.FightBehavior
{
    public readonly struct FightSpecs
    {
        public float Health { get; }
        public float AverageDamagePerSecond { get; }

        public FightSpecs(float health, float averageDamagePerSecond)
        {
            if (health <= 0 || averageDamagePerSecond <= 0) throw new ArgumentException("Unit has invalid specs");

            Health = health;
            AverageDamagePerSecond = averageDamagePerSecond;
        }

        public float WouldWinInTime(FightSpecs opponent, float advanceTime)
        {
            if (opponent.Health - (AverageDamagePerSecond * advanceTime) > 0)
                return float.NegativeInfinity;

            return opponent.Health / AverageDamagePerSecond;
        }

        public float WouldLoseInTime(FightSpecs opponent, List<FightSpecs> surroundingOpponents, float advanceTime)
        {
            var averageDamagePerSecond = opponent.AverageDamagePerSecond +
                                         surroundingOpponents.Sum(surroundingOpponent =>
                                             surroundingOpponent.AverageDamagePerSecond);

            if (Health - (averageDamagePerSecond * advanceTime) > 0)
                return float.NegativeInfinity;

            return Health / opponent.AverageDamagePerSecond;
        }
    }
}
