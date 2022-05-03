using Units.Enums;
using static Units.MinimaxBehavior.Fight.FightState;

namespace Units.MinimaxBehavior
{
    public class MinimaxScoreFunction
    {
        public static float Calculate(FightCondition fightCondition, Player player)
        {
            if (player.Fraction == Fraction.Colonists)
            {
                return CalculateForFirstPlayer(fightCondition);
            }
            else
            {
                return CalculateForSecondPlayer(fightCondition);
            }
        }

        private static float CalculateForFirstPlayer(FightCondition fightCondition)
        {
            return fightCondition.State switch
            {
                FirstPlayerVictory => 1f,
                SecondPlayerVictory => -1f,
                _ => 0f
            };
        }
        
        private static float CalculateForSecondPlayer(FightCondition fightCondition)
        {
            return fightCondition.State switch
            {
                FirstPlayerVictory => -1f,
                SecondPlayerVictory => 1f,
                _ => 0f
            };
        }
    }
}
