using System;
using NPBehave;
using ResourceManagement;
using UnitManagement.Targeting;

namespace Units.Unit.BehaviorNodes
{
    public class StartActionOnTarget : Node
    {
        private readonly string _targetKey;
        private readonly string _unitTypeKey;
        
        private Resource _resource;

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

            if (!CanAct(target))
            {
                return;
            }

            Act();
        }

        private bool CanAct(Target target)
        {
            if (!target.HasResource)
            {
                Stopped(false);
                return false;
            }

            _resource = target.Resource;
            var unitType = Blackboard.Get<UnitType.UnitType>(_unitTypeKey);

            if (_resource.CanInteractWith(unitType))
            {
                Stopped(false);
                return false;
            }

            return true;
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
