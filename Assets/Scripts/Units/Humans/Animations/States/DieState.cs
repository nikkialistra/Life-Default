using System;

namespace Units.Humans.Animations.States
{
    public class DieState : HumanState
    {
        public Action EndAction { get; set; }
        
        public override AnimationType AnimationType => AnimationType.Die;

        public override bool CanEnterState => true;

        public override void OnEnterState()
        {
            base.OnEnterState();
            
            _clip.Events.OnEnd = EndAction;
        }

        public override void OnExitState()
        {
            _clip.Events.OnEnd = null;
        }
    }
}
