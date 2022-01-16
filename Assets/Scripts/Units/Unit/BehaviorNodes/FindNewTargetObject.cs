using NPBehave;

namespace Units.Unit.BehaviorNodes
{
    public class FindNewTargetObject : Node
    {
        public FindNewTargetObject() : base("FindNewTarget")
        {
            
        }
        
        protected override void DoStart()
        {
            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
