using Sirenix.OdinInspector;
using Units.Humans;
using Units.Humans.Animations.States;
using UnityEngine;

namespace Colonists
{
    public class ColonistAnimations : MonoBehaviour
    {
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

        [Required]
        [SerializeField] private GatherResourceState _cutTreesState;
        [Required]
        [SerializeField] private GatherResourceState _mineRocksState;

        public void Idle()
        {
            _humanAnimations.Idle();
        }
        
        public void MineRocks()
        {
            _humanAnimations.StateMachine.TrySetState(_mineRocksState);
        }

        public void CutTrees()
        {
            _humanAnimations.StateMachine.TrySetState(_cutTreesState);
        }

        public void StopGathering()
        {
            _humanAnimations.ForceIdle();
        }
    }
}
