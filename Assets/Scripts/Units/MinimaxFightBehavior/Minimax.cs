using System;
using System.Linq;
using Units.MinimaxFightBehavior.FightAct;

namespace Units.MinimaxFightBehavior
{
    public class Minimax
    {
        public static FightMove FindBestMove(Fight fight, int? maxDepth)
        {
            if (fight.IsTerminal)
            {
                throw new InvalidOperationException("Cannot find best move for finished fight");
            }

            var activePlayer = fight.ActivePlayer;

            if (activePlayer == null)
            {
                throw new InvalidOperationException("Cannot find best move for not existing player");
            }

            var (_, nextMove) = Estimate(fight, activePlayer, maxDepth);

            return nextMove;
        }

        private static (float Score, FightMove NextMove) Estimate(Fight fight, Player player, int? depth)
        {
            if (fight.IsTerminal || depth is <= 0)
            {
                return (Score: MinimaxScoreFunction.Calculate(fight, player),
                    NextMove: null);
            }

            var possibleMoves = fight.GetPossibleMoves();

            if (possibleMoves.Count == 0)
            {
                throw new InvalidOperationException("Cannot find moves for not terminal fight condition");
            }

            var estimations = possibleMoves.Select((move) =>
            {
                fight.MakeMove(move);
                var (score, _) = Estimate(fight, player, depth - 1);
                fight.UndoMove();

                return (Score: score, NextMove: move);
            });

            var isOpponentTurn = player != fight.ActivePlayer;
            
            return isOpponentTurn
                ? estimations.Aggregate((e1, e2) => e1.Score < e2.Score ? e1 : e2)
                : estimations.Aggregate((e1, e2) => e1.Score >= e2.Score ? e1 : e2);
        }
    }
}
