using NPBehave;
using UnitManagement.Targeting;

namespace Units.Unit.BehaviorNodes
{
    public class StartActionOnTargetObject : Node
    {
        private readonly string _targetKey;

        public StartActionOnTargetObject(string targetKey) : base("StartActionOnTarget")
        {
            _targetKey = targetKey;
        }
        
        protected override void DoStart()
        {
            var target = Blackboard.Get<Target>(_targetKey);
            
            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
