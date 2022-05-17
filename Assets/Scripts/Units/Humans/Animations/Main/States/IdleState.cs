using Animancer.FSM;

namespace Units.Humans.Animations.Main.States
{
    public class IdleState : MainHumanState
    {
        public override MainAnimationType MainAnimationType => MainAnimationType.Idle;

        public override bool CanEnterState
        {
            get
            {
                if (StateChange<MainHumanState>.PreviousState == null)
                {
                    return true;
                }
                
                return StateChange<MainHumanState>.PreviousState.MainAnimationType switch
                {
                    MainAnimationType.Move => true,
                    _ => false
                };
            }
        }
    }
}
