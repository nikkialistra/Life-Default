namespace Units.MinimaxFightBehavior.FightAct
{
    public class FightMove
    {
        public FightMove(float hitDamage, float takeDamage)
        {
            HitDamage = hitDamage;
            TakeDamage = takeDamage;
        }

        public float HitDamage { get; }
        public float TakeDamage { get; }

        public static FightMove GetReverseState(FightMove fightMove)
        {
            return new FightMove(-fightMove.HitDamage, -fightMove.TakeDamage);
        }
    }
}
