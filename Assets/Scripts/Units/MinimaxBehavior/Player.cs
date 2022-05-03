using System.Collections.Generic;
using Units.Enums;

namespace Units.MinimaxBehavior
{
    public class Player
    {
        private float _hitDamage;
        private float _missChance;
        
        private float _selfDamage;
        private float _dodgeChance;

        public Player(Fraction fraction, float hitDamage, float missChance, float selfDamage, float dodgeChance)
        {
            Fraction = fraction;

            _hitDamage = hitDamage;
            _missChance = missChance;

            _selfDamage = selfDamage;
            _dodgeChance = dodgeChance;
        }
        
        public Fraction Fraction { get; }

        public List<FightMove> GetPossibleMoves()
        {
            return new List<FightMove>()
            {
                new (_hitDamage, _missChance, _selfDamage, _dodgeChance),
                new (0f, 0f, 0f, 0f)
            };
        }
    }
}
