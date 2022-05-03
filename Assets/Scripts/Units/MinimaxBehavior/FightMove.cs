namespace Units.MinimaxBehavior
{
    public class FightMove
    {
        public FightMove(float hitDamage, float missChance, float selfDamage, float dodgeChance)
        {
            HitDamage = hitDamage;
            MissChance = missChance;

            SelfDamage = selfDamage;
            DodgeChance = dodgeChance;
        }

        public float HitDamage { get; }
        public float MissChance { get; }
        
        public float SelfDamage { get; }
        public float DodgeChance { get; }

        public static FightMove GetReverseState(FightMove fightMove)
        {
            return new FightMove(-fightMove.HitDamage, fightMove.MissChance, -fightMove.SelfDamage,
                fightMove.DodgeChance);
        }
    }
}
