using System.Collections.Generic;

namespace Units.MinimaxFightBehavior.FightAct
{
    public class Player
    {
        private readonly float _hitDamage;
        private readonly float _selfDamage;

        public Player(PlayerSide side, float startHealth, float hitDamage, float selfDamage)
        {
            Side = side;

            StartHealth = startHealth;
            
            _hitDamage = hitDamage;
            _selfDamage = selfDamage;
        }

        public enum PlayerSide
        {
            First,
            Second
        }
        
        public PlayerSide Side { get; }

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
