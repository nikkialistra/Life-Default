using System;
using NPBehave;
using UnitManagement.Targeting;

namespace Units.Unit.BehaviorNodes
{
    public class StartActionOnTarget : Node
    {
        private readonly string _targetKey;
        private readonly string _unitTypeKey;

        public StartActionOnTarget(string targetKey, string unitTypeKey) : base("StartActionOnTarget")
        {
            _targetKey = targetKey;
            _unitTypeKey = unitTypeKey;
        }
        
        protected override void DoStart()
        {
            var target = Blackboard.Get<TargetMark>(_targetKey).Target;

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Act();
        }

        private void Act()
        {
            
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
