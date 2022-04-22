using UnityEngine;

namespace Colonists.StateMachineBehaviours
{
    public class ResourceGatheringBehaviour : StateMachineBehaviour
    {
        [SerializeField] private float _hitTime = 0.7f;
        
        private ColonistGatherer _colonistGatherer;
        private UnitEquipment _unitEquipment;
        
        private float _nextHitTime;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _colonistGatherer = animator.transform.parent.GetComponent<ColonistGatherer>();
            _unitEquipment = animator.GetComponent<UnitEquipment>();
            _nextHitTime = _hitTime;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime > _nextHitTime)
            {
                _colonistGatherer.Hit(stateInfo.length);
                _nextHitTime++;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _unitEquipment.Unequip();
        }
    }
}
