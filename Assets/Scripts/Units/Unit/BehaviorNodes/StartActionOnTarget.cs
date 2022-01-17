using NPBehave;
using UnitManagement.Targeting;

namespace Units.Unit.BehaviorNodes
{
    public class StartActionOnTarget : Node
    {
        private readonly string _targetKey;

        public StartActionOnTarget(string targetKey) : base("StartActionOnTarget")
        {
            _targetKey = targetKey;
        }
        
        protected override void DoStart()
        {
            var target = Blackboard.Get<TargetMark>(_targetKey);
            
            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
