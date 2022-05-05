using BehaviorDesigner.Runtime;
using Units;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(BehaviorTree))]
    [RequireComponent(typeof(UnitAttacker))]
    public class EnemyBehavior : MonoBehaviour
    {
        private bool _initialized;

        private SharedBool _newCommand;
        
        private BehaviorTree _behaviorTree;

        private UnitAttacker _unitAttacker;

        private void Awake()
        {
            _behaviorTree = GetComponent<BehaviorTree>();
            _unitAttacker = GetComponent<UnitAttacker>();
        }
        
        private void OnEnable()
        {
            _unitAttacker.WantEscape += Escape;
        }

        private void OnDisable()
        {
            _unitAttacker.WantEscape -= Escape;
        }

        public void Enable()
        {
            _behaviorTree.EnableBehavior();
        }

        public void Disable()
        {
            _behaviorTree.DisableBehavior();
        }
        
        public void Activate()
        {
            if (!_initialized)
            {
                Initialize();
            }

            _behaviorTree.EnableBehavior();
        }
        
        public void Deactivate()
        {
            _behaviorTree.DisableBehavior();
        }
        
        private void Initialize()
        {
            _newCommand = (SharedBool)_behaviorTree.GetVariable("NewCommand");
        }

        private void Escape()
        {
            
        }
    }
}
