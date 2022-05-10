using Animancer.FSM;

namespace Units.Humans.Animations.States
{
    public class AttackState : HumanState
    {
        public float Speed
        {
            set => _clip.Speed = value;
        }
        
        public override AnimationType AnimationType => AnimationType.Attack;

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
