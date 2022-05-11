using System;
using Animancer.FSM;

namespace Units.Humans.Animations.States
{
    public class IdleState : HumanState
    {
        public event Action Enter;
        
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

        public override void OnEnterState()
        {
            base.OnEnterState();
            
            Enter?.Invoke();
        }
    }
}
