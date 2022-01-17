using NPBehave;
using UnitManagement.Targeting;

namespace Units.Unit.BehaviorNodes
{
    public class CheckHasTarget : Node
    {
        private readonly string _targetMarkKey;

        public CheckHasTarget(string targetMarkKey) : base("CheckHasTarget")
        {
            _targetMarkKey = targetMarkKey;
        }

        protected override void DoStart()
        {
            var target = Blackboard.Get<TargetMark>(_targetMarkKey);
            
            Stopped(target.HasTarget);
        }
        
        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
