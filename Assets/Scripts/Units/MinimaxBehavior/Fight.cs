using System.Collections.Generic;

namespace Units.MinimaxBehavior
{
    public class Fight
    {
        private readonly Player _firstPlayer;
        private readonly Player _secondPlayer;

        public Fight(Player firstPlayer, Player secondPlayer)
        {
            _firstPlayer = firstPlayer;
            _secondPlayer = secondPlayer;
        }
        
        public enum FightState
        {
            FirstPlayerTurn,
            SecondPlayerTurn,
            
            FirstPlayerVictory,
            SecondPlayerVictory,
            
            Draw
        }

        public Player FirstPlayer => _firstPlayer;
        public Player SecondPlayer => _secondPlayer;

        public FightState State { get; private set; }

        public List<FightMove> GetPossibleMoves()
        {
            throw new System.NotImplementedException();
        }

        public void MakeMove(Player activePlayer, FightMove fightMove)
        {
            throw new System.NotImplementedException();
        }

        public void UndoMove()
        {
            throw new System.NotImplementedException();
        }
    }
}
