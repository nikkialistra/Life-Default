using NPBehave;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class ResetBehavior : Node
    {
        private readonly string _newCommandKey;
        private readonly UnitClass _unitClass;

        public ResetBehavior(string newCommandKey, UnitClass unitClass) : base("ResetBehavior")
        {
            _newCommandKey = newCommandKey;
            _unitClass = unitClass;
        }

        protected override void DoStart()
        {
            Reset();
            Stopped(true);
        }

        private void Reset()
        {
            Blackboard.Unset(_newCommandKey);

            _unitClass.StopInteraction();
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
