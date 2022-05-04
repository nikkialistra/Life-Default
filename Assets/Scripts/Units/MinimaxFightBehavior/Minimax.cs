using System;
using System.Linq;
using Units.MinimaxFightBehavior.FightAct;
using UnityEngine;

namespace Units.MinimaxFightBehavior
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

            var (_, nextMove) = Estimate(fight, activePlayer, _currentDepth);

            return nextMove;
        }

        private (float Score, FightMove NextMove) Estimate(Fight fight, Player player, int depth)
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
                var (score, _) = Estimate(fight, player, nextDepth);
                fight.UndoMove();
                
                LogScore(score, depth);

                return (Score: score, NextMove: move);
            });

            var isOpponentTurn = player != fight.ActivePlayer;

            var estim2 = isOpponentTurn
                ? estimations.Aggregate((e1, e2) => e1.Score <= e2.Score ? e1 : e2)
                : estimations.Aggregate((e1, e2) => e1.Score >= e2.Score ? e1 : e2);

            return estim2;
        }

        private bool ShouldNotLookFurther(Fight fight, Player player, int depth,
            out (float Score, FightMove NextMove) estimation)
        {
            if (fight.IsTerminal)
            {
                {
                    estimation = (Score: MinimaxScoreFunction.Calculate(fight, player),
                        NextMove: null);
                    return true;
                }
            }

            if (depth >= _maxDepth)
            {
                var score = MinimaxScoreFunction.Calculate(fight, player);

                LogScore(score, depth);

                var defaultMove = GetDefaultMove(fight);

                {
                    estimation = (Score: score,
                        NextMove: defaultMove);
                    return true;
                }
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
    }
}
