using NPBehave;

namespace Units.Unit.BehaviorNodes
{
    public class FindNewTarget : Node
    {
        public FindNewTarget() : base("FindNewTarget")
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
