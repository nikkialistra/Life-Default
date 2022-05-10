namespace Units.Humans.Animations.States
{
    public class DieState : HumanState
    {
        public override AnimationType AnimationType => AnimationType.Die;

        public override bool CanEnterState => true;
    }
}
