using Units.Enums;

namespace Units.MinimaxFightBehavior.FightAct
{
    public readonly struct HistoryFightState
    {
        public HistoryFightState(Fight.FightState state, Player.PlayerSide whichMove, FightMove fightMove)
        {
            State = state;
            WhichMove = whichMove;
            FightMove = fightMove;
        }
        
        public Fight.FightState State { get; }
        public Player.PlayerSide WhichMove { get; }

        public FightMove ReverseFightMove => FightMove.GetReverseState(FightMove);
        private FightMove FightMove { get; }
    }
}
