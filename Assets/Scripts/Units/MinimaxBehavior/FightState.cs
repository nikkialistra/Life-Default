using System;
using System.Collections.Generic;
using static Units.MinimaxBehavior.Fight.FightState;

namespace Units.MinimaxBehavior
{
    public class FightState
    {
        private readonly Fight _fight;
        
        public FightState(Fight fight)
        {
            _fight = fight;
        }

        public bool IsTerminal => _fight.State switch
        {
            FirstPlayerVictory or SecondPlayerVictory or Draw => true,
            _ => false
        };

        public Player ActivePlayer => _fight.State switch
        {
            FirstPlayerTurn => _fight.FirstPlayer,
            SecondPlayerTurn => _fight.SecondPlayer,
            _ => null
        };
        
        public List<FightMove> GetPossibleMoves()
        {
            return _fight.GetPossibleMoves();
        }

        public void MakeMove(FightMove fightMove)
        {
            if (ActivePlayer == null)
            {
                throw new InvalidOperationException("Cannot make move when there is no turns");
            }

            _fight.MakeMove(ActivePlayer, fightMove);
        }

        public void UndoMove()
        {
            _fight.UndoMove();
        }
    }
}
