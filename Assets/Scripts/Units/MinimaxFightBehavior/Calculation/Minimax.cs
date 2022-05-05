using System;
using System.Collections.Generic;
using System.Linq;
using Units.MinimaxFightBehavior.FightAct;
using UnityEngine;

namespace Units.MinimaxFightBehavior.Calculation
{
    public class Minimax
    {
        private readonly int _maxDepth;
        private readonly bool _shouldLog;

        private int _currentDepth;

        public Minimax(int maxDepth, bool shouldLog)
        {
            _maxDepth = maxDepth;
            _shouldLog = shouldLog;
        }
        
        public FightMove FindBestMove(Fight fight)
        {
            _currentDepth = 1;
            
            if (fight.IsTerminal)
            {
                throw new InvalidOperationException("Cannot find best move for finished fight");
            }

            var activePlayer = fight.ActivePlayer;

            if (activePlayer == null)
            {
                throw new InvalidOperationException("Cannot find best move for not existing player");
            }

            var (_, nextMove, _) = Estimate(fight, activePlayer, _currentDepth);

            return nextMove;
        }

        private EstimationResult Estimate(Fight fight, Player player, int depth)
        {
            if (ShouldNotLookFurther(fight, player, depth, out var estimation))
            {
                return estimation;
            }

            var possibleMoves = fight.GetPossibleMoves();

            if (possibleMoves.Count == 0)
            {
                throw new InvalidOperationException("Cannot find moves for not terminal fight condition");
            }

            var estimations = possibleMoves.Select(move =>
            {
                var nextDepth = depth + 1;
                
                fight.MakeMove(move);
                var (score, _, shouldCancel) = Estimate(fight, player, nextDepth);
                fight.UndoMove();
                
                LogScore(score, depth);

                return new EstimationResult(score, move, shouldCancel);
            }).ToList();

            if (depth == 1)
            {
                estimations = FilterEstimationsWithDefeat(estimations);
            }

            var isOpponentTurn = player != fight.ActivePlayer;

            if (depth != 1 && estimations.Any(item => item.ShouldCancel))
            {
                return new EstimationResult(0, null, true);
            }
            
            return isOpponentTurn
                ? estimations.Aggregate((e1, e2) => e1.Score <= e2.Score ? e1 : e2)
                : estimations.Aggregate((e1, e2) => e1.Score >= e2.Score ? e1 : e2);
        }

        private static List<EstimationResult> FilterEstimationsWithDefeat(
            List<EstimationResult> estimations)
        {
            return estimations.Where(estimation => estimation.ShouldCancel != true).ToList();
        }

        private bool ShouldNotLookFurther(Fight fight, Player player, int depth,
            out EstimationResult estimation)
        {
            if (fight.IsTerminal)
            {
                {
                    var score = MinimaxScoreFunction.Calculate(fight, player);

                    LogScore(score, depth);

                    estimation = new EstimationResult(score, null, ShouldCancel(score));
                    return true;
                }
            }

            if (depth >= _maxDepth)
            {
                var score = MinimaxScoreFunction.Calculate(fight, player);

                LogScore(score, depth);

                var defaultMove = GetDefaultMove(fight);

                estimation = new EstimationResult(score, defaultMove, false);
                return true;
            }

            estimation = default;
            return false;
        }

        private static FightMove GetDefaultMove(Fight fight)
        {
            var possibleMoves = fight.GetPossibleMoves();

            if (possibleMoves.Count == 0)
            {
                throw new InvalidOperationException("Cannot find default move for not terminal fight condition");
            }
            
            return fight.GetPossibleMoves()[0];
        }

        private void LogScore(float score, int depth)
        {
            if (_shouldLog)
            {
                Debug.Log(new string('-', depth) + $" {score}");
            }
        }

        private static bool ShouldCancel(float score)
        {
            return score == -1;
        }
    }
}
