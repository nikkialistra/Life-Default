using System.Reflection;
using Sirenix.OdinInspector;
using Units.Enums;
using Units.MinimaxFightBehavior.FightAct;
using UnityEngine;

namespace Units.MinimaxFightBehavior
{
    public class VirtualFight : MonoBehaviour
    {
        [SerializeField] private bool _shouldLog;
        
        [Space]
        [SerializeField] private int _maxFightMoveCount = 30;
        [SerializeField] private int _maxMinimaxDepth = 3;

        [SerializeField] private int _startHealths = 100;

        private Fight _fight;

        private int _moveCount;

        [Button(ButtonSizes.Medium)]
        public void StartFight()
        {
            ClearLog();
            
            _moveCount = 0;
            
            var minimax = new Minimax(_maxMinimaxDepth, _shouldLog);
            
            var colonist = new Player(Fraction.Colonists,  _startHealths, 30f, 20f);
            var enemy = new Player(Fraction.Enemies, _startHealths, 20f, 30f);

            _fight = new Fight(colonist, enemy);

            ShowStatus();
            
            while (!_fight.IsTerminal && _moveCount < _maxFightMoveCount)
            {
                var bestMove = minimax.FindBestMove(_fight);

                ShowMove(bestMove);
                
                _fight.MakeMove(bestMove);
                
                ShowStatus();

                _moveCount++;
            }
        }

        private void ShowMove(FightMove bestMove)
        {
            if (_shouldLog)
            {
                Debug.Log(
                $"Player from {_fight.ActivePlayer.Fraction} chooses to hit {bestMove.HitDamage} and take {bestMove.TakeDamage}");
            }
        }

        private void ShowStatus()
        {
            if (_shouldLog)
            {
                _fight.ShowCurrentFightStatus();
            }
        }

        private static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
    }
}
