using System.Collections.Generic;
using Units.Enums;

namespace Units.MinimaxFightBehavior.FightAct
{
    public class Player
    {
        private readonly float _hitDamage;
        private readonly float _selfDamage;

        public Player(Fraction fraction, float startHealth, float hitDamage, float selfDamage)
        {
            Fraction = fraction;

            StartHealth = startHealth;
            
            _hitDamage = hitDamage;
            _selfDamage = selfDamage;
        }
        
        public Fraction Fraction { get; }

        public float StartHealth { get; }

        public List<FightMove> GetPossibleMoves()
        {
            return new List<FightMove>()
            {
                new (_hitDamage, _selfDamage),
                new (1000f, 1000f)
            };
        }
    }
}
