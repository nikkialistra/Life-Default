using Sirenix.OdinInspector;
using Units.Humans;
using Units.Humans.Animations.Actions.States;
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

        public void MineRocks()
        {
            _humanAnimations.TrySetActionsState(_mineRocksState);
        }

        public void CutTrees()
        {
            _humanAnimations.TrySetActionsState(_cutTreesState);
        }

        public void StopGathering()
        {
            _humanAnimations.StopActions();
        }
    }
}
