using NPBehave;

namespace Units.Unit.BehaviorNodes
{
    public class CheckHasTarget : Node
    {
        private readonly string _targetKey;

        public CheckHasTarget(string targetKey) : base("CheckHasTarget")
        {
            _targetKey = targetKey;
        }

        protected override void DoStart()
        {
            Stopped(Blackboard.Isset(_targetKey));
        }
        
        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
