using NPBehave;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class ResetBehavior : Node
    {
        private readonly string _newCommandKey;
        private readonly string _unitClassKey;

        public ResetBehavior(string newCommandKey, string unitClassKey) : base("ClearCommand")
        {
            _newCommandKey = newCommandKey;
            _unitClassKey = unitClassKey;
        }

        protected override void DoStart()
        {
            Reset();
            Stopped(true);
        }

        private void Reset()
        {
            Blackboard.Unset(_newCommandKey);

            var unitClass = Blackboard.Get<UnitClass>(_unitClassKey);
            unitClass.StopInteraction();
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
