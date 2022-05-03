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

        private int _moveCount;

        [Button(ButtonSizes.Medium)]
        public void StartFight()
        {
            _moveCount = 0;
            
            var colonist = new Player(Fraction.Colonists,  100f, 30f, 20f);
            var enemy = new Player(Fraction.Enemies, 100f, 20f, 30f);

            var fight = new Fight(colonist, enemy);

            fight.ShowCurrentFightStatus();
            
            while (!fight.IsTerminal && _moveCount < _maxFightMoveCount)
            {
                var bestMove = Minimax.FindBestMove(fight, _maxMinimaxDepth);

                Debug.Log($"Player from {fight.ActivePlayer.Fraction} chooses to hit {bestMove.HitDamage} and take {bestMove.TakeDamage}");
                
                fight.MakeMove(bestMove);
                
                fight.ShowCurrentFightStatus();

                _moveCount++;
            }
        }
    }
}
