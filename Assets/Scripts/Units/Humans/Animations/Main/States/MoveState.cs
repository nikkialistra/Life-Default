using Animancer.FSM;
using Units.Humans.Animations.Main;
using Units.Humans.Animations.Main.States;

namespace Units.Humans.Animations.States
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
