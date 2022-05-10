using Animancer.FSM;

namespace Units.Humans.Animations.States
{
    public class GatherResourceState : HumanState
    {
        public override AnimationType AnimationType => AnimationType.GatherResource;
        
        public override bool CanEnterState
        {
            get
            {
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Die => false,
                    _ => true
                };
            }
        }
    }
}
