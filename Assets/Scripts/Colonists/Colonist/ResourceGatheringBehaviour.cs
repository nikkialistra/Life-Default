using UnityEngine;

namespace Colonists.Colonist
{
    public class ResourceGatheringBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var colonistHandEquipment = animator.GetComponent<ColonistHandEquipment>();
            colonistHandEquipment.Unequip();
        }
    }
}
