using Humans.Animations;
using Humans.Animations.States;
using Sirenix.OdinInspector;
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
            _humanAnimations.TrySetState(_mineRocksState);
        }

        public void CutTrees()
        {
            _humanAnimations.TrySetState(_cutTreesState);
        }

        public void StopGathering()
        {
            _humanAnimations.StopActions();
        }
    }
}
