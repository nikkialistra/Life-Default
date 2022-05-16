using Animancer.FSM;
using UnityEngine;

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
                    Debug.Log(1);
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
