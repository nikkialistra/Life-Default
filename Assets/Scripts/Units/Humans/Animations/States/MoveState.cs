using System;
using Animancer.FSM;

namespace Units.Humans.Animations.States
{
    public class MoveState : HumanState
    {
        public event Action Enter;
        
        public override AnimationType AnimationType => AnimationType.Move;
        
        public override bool CanEnterState
        {
            get
            {
                return StateChange<HumanState>.PreviousState.AnimationType switch
                {
                    AnimationType.Idle => true,
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
