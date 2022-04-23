using Colonists;
using UnityEngine;

namespace Units.StateMachineBehaviours
{
    public class AttackingBehaviour : StateMachineBehaviour
    {
        [SerializeField] private float _hitTime = 0.7f;
        
        private UnitAttacker _unitAttacker;

        private float _nextHitTime;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 1);
            
            _unitAttacker = animator.transform.parent.GetComponent<UnitAttacker>();
            _nextHitTime = _hitTime;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime > _nextHitTime)
            {
                _unitAttacker.Hit(stateInfo.length);
                _nextHitTime++;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 0);
        }
    }
}
