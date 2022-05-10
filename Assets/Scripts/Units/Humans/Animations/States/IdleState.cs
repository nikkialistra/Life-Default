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
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Move => true,
                    _ => false
                };
            }
        }
    }
}
