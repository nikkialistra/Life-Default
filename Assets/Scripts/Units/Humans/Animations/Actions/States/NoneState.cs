namespace Units.Humans.Animations.Actions.States
{
    public class NoneState : ActionsHumanState
    {
        public override ActionsAnimationType ActionsAnimationType => ActionsAnimationType.None;

        public override void OnEnterState()
        {
            _animancer.Layers[AnimationLayers.Actions].StartFade(0f, _clip.FadeDuration);
        }

        public override void OnExitState()
        {
            _animancer.Layers[AnimationLayers.Actions].Weight = 1;
        }
    }
}
