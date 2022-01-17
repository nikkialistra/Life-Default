using NPBehave;

namespace Units.Unit.BehaviorNodes
{
    public class ClearCommand : Node
    {
        private readonly string _newCommandKey;

        public ClearCommand(string newCommandKey) : base("ClearCommand")
        {
            _newCommandKey = newCommandKey;
        }

        protected override void DoStart()
        {
            Blackboard.Unset(_newCommandKey);
            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
