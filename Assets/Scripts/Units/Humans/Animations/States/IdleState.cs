using Animancer.FSM;

namespace Units.Humans.Animations.States
{
    public class IdleState : HumanState
    {
        public override AnimationType AnimationType => AnimationType.Idle;

        public override bool CanEnterState
        {
            get
            {
                if (StateChange<HumanState>.PreviousState == null)
                {
                    return true;
                }
                
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Move => true,
                    _ => false
                };
            }
        }
    }
}
