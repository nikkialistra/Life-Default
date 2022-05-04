using Sirenix.OdinInspector;
using Units.Enums;
using Units.MinimaxFightBehavior.FightAct;
using UnityEngine;

namespace Units.MinimaxFightBehavior
{
    public class VirtualFight : MonoBehaviour
    {
        [SerializeField] private int _maxFightMoveCount = 30;
        [SerializeField] private int _maxMinimaxDepth = 3;

        [SerializeField] private int _startHealths = 100;
        

        private int _moveCount;

        [Button(ButtonSizes.Medium)]
        public void StartFight()
        {
            _moveCount = 0;
            
            var colonist = new Player(Fraction.Colonists,  _startHealths, 30f, 20f);
            var enemy = new Player(Fraction.Enemies, _startHealths, 20f, 30f);

            var fight = new Fight(colonist, enemy);
            var minimax = new Minimax(_maxMinimaxDepth);

            fight.ShowCurrentFightStatus();
            
            while (!fight.IsTerminal && _moveCount < _maxFightMoveCount)
            {
                var bestMove = minimax.FindBestMove(fight);

                Debug.Log($"Player from {fight.ActivePlayer.Fraction} chooses to hit {bestMove.HitDamage} and take {bestMove.TakeDamage}");
                
                fight.MakeMove(bestMove);
                
                fight.ShowCurrentFightStatus();

                _moveCount++;
            }
        }
    }
}
