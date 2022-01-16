using NPBehave;
using UnitManagement.Targeting;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class CheckHasTargetObject : Node
    {
        private readonly string _targetKey;

        public CheckHasTargetObject(string targetKey) : base("CheckHasTarget")
        {
            _targetKey = targetKey;
        }

        protected override void DoStart()
        {
            var target = Blackboard.Get<Target>(_targetKey);
            
            Stopped(target.HasTargetObject);
        }
        
        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
