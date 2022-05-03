using System;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

namespace Units.MinimaxBehavior
{
    public class Minimax
    {
        public FightMove FindBestMove(FightCondition fightCondition, int? maxDepth)
        {
            if (fightCondition.IsTerminal)
            {
                throw new InvalidOperationException("Cannot find best move for finished fight");
            }

            var activePlayer = fightCondition.ActivePlayer;

            if (activePlayer == null)
            {
                throw new InvalidOperationException("Cannot find besh move for not existing player");
            }

            var (_, nextMove) = Estimate(fightCondition, activePlayer, maxDepth);

            return nextMove;
        }

        private (float Score, FightMove NextMove) Estimate(FightCondition fightCondition, Player player, int? depth)
        {
            if (fightCondition.IsTerminal || depth is <= 0)
            {
                return (Score: MinimaxScoreFunction.Calculate(fightCondition, player),
                    NextMove: null);
            }

            var possibleMoves = fightCondition.GetPossibleMoves();

            if (possibleMoves.Count == 0)
            {
                throw new InvalidOperationException("Cannot find moves for not terminal fight condition");
            }

            var estimations = possibleMoves.Select((move) =>
            {
                fightCondition.MakeMove(move);
                var (score, _) = Estimate(fightCondition, player, depth - 1);
                fightCondition.UndoMove();

                return (Score: score, NextMove: move);
            });

            var isOpponentTurn = player != fightCondition.ActivePlayer;
            
            return isOpponentTurn
                ? estimations.Aggregate((e1, e2) => e1.Score < e2.Score ? e1 : e2)
                : estimations.Aggregate((e1, e2) => e1.Score > e2.Score ? e1 : e2);
        }
    }
}
