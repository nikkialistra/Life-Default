using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Units.Humans.Animations.Main.States
{
    public class MoveState : MainHumanState
    {
        public override MainAnimationType MainAnimationType => MainAnimationType.Move;

        public override bool CanEnterState
        {
            get
            {
                return StateChange<MainHumanState>.PreviousState.MainAnimationType switch
                {
                    MainAnimationType.Idle => true,
                    _ => false
                };
            }
        }
    }
}
