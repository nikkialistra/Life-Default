using Units.MinimaxFightBehavior.FightAct;

namespace Units.MinimaxFightBehavior.Calculation
{
    public readonly struct EstimationResult
    {
        public EstimationResult(float score, FightMove fightMove, bool shouldCancel)
        {
            Score = score;
            FightMove = fightMove;
            ShouldCancel = shouldCancel;
        }

        public float Score { get; }
        public FightMove FightMove { get; }
        public bool ShouldCancel { get; }

        public void Deconstruct(out float score, out FightMove fightMove, out bool shouldCancel)
        {
            score = Score;
            fightMove = FightMove;
            shouldCancel = ShouldCancel;
        }
    }
}
