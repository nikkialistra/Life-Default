using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(BehaviorTree))]
    public class EnemyBehavior : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;

        private bool _initialized;

        private void Awake()
        {
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        public void Enable()
        {
            _behaviorTree.EnableBehavior();
        }

        public void Disable()
        {
            _behaviorTree.DisableBehavior();
        }
    }
}
