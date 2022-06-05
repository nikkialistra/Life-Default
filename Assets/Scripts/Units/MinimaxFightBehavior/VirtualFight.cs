using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Units.FightBehavior;
using Units.MinimaxFightBehavior.Calculation;
using Units.MinimaxFightBehavior.FightAct;
using UnityEngine;
using static Units.MinimaxFightBehavior.FightAct.Player.PlayerSide;

namespace Units.MinimaxFightBehavior
{
    public class VirtualFight : MonoBehaviour
    {
        [SerializeField] private bool _shouldLog;
        [SerializeField] private bool _shouldLogInDetail;
        
        [Space]
        [SerializeField] private int _maxFightMoveCount = 30;
        [SerializeField] private int _maxMinimaxDepth = 3;

        [Title("Test Fight")]
        [SerializeField] private int _startHealths = 100;

        private Fight _fight;

        private int _moveCount;
        
        private Player _firstPlayer;
        private Player _secondPlayer;
        
        private bool _defeatOnFirstMove;

        [Button(ButtonSizes.Medium)]
        private void StartTestFight()
        {
            CreateExamplePlayers();
            StartFight(_maxMinimaxDepth);
        }

        public bool CheckDefeatForFight(FightSpecs selfSpecs, FightSpecs opponentSpecs,
            List<FightSpecs> surroundingOpponentsSpecs, int depth)
        {
            _defeatOnFirstMove = false;

            CreatePlayersFrom(selfSpecs, opponentSpecs, surroundingOpponentsSpecs);
            StartFight(depth);

            return _defeatOnFirstMove;
        }

        private void CreatePlayersFrom(FightSpecs firstSpecs, FightSpecs secondSpecs, List<FightSpecs> surroundingSecondSpecs)
        {
            var secondPlayerAverageDamagePerSecond = secondSpecs.AverageDamagePerSecond +
                                         surroundingSecondSpecs.Sum(surroundingOpponent => surroundingOpponent.AverageDamagePerSecond);

            _firstPlayer = new Player(First, firstSpecs.Health, firstSpecs.AverageDamagePerSecond,
                secondPlayerAverageDamagePerSecond);
            _secondPlayer = new Player(Second, secondSpecs.Health, secondPlayerAverageDamagePerSecond,
                firstSpecs.AverageDamagePerSecond);
        }

        private void StartFight(int depth)
        {
#if UNITY_EDITOR
            ClearLog();
#endif
            
            _moveCount = 0;
            _fight = new Fight(_firstPlayer, _secondPlayer);
            
            var minimax = new Minimax(depth, _shouldLogInDetail);

            ShowStatus();
            
            while (!_fight.IsTerminal && _moveCount < _maxFightMoveCount)
            {
                var bestMove = minimax.FindBestMove(_fight);

                if (_moveCount == 0)
                {
                    CheckDefeatOnFirstMove(bestMove);
                }

                ShowMove(bestMove);
                
                _fight.MakeMove(bestMove);
                
                ShowStatus();

                _moveCount++;
            }
        }

        private void CheckDefeatOnFirstMove(FightMove move)
        {
            if (move.HitDamage == 1000f && move.TakeDamage == 1000f)
            {
                _defeatOnFirstMove = true;
            }
        }

        private void CreateExamplePlayers()
        {
            _firstPlayer = new Player(First, _startHealths, 10f, 20f);
            _secondPlayer = new Player(Second, _startHealths, 30f, 30f);
        }

        private void ShowMove(FightMove bestMove)
        {
            if (_shouldLog)
            {
                if (bestMove.HitDamage == 1000f && bestMove.TakeDamage == 1000f)
                {
                    Debug.Log($"Player from {_fight.ActivePlayer.Side} chooses to accept a draw");
                }
                else
                {
                    Debug.Log($"Player from {_fight.ActivePlayer.Side} chooses to hit {bestMove.HitDamage} and take {bestMove.TakeDamage}");
                }
            }
        }

        private void ShowStatus()
        {
            if (_shouldLog)
            {
                _fight.ShowCurrentFightStatus();
            }
        }

#if UNITY_EDITOR
        private static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif
    }
}
