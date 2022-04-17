using UnityEngine;

namespace Colonists
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
