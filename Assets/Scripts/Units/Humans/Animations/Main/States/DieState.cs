using System;
using Units.Humans.Animations.Main;
using Units.Humans.Animations.Main.States;

namespace Units.Humans.Animations.States
{
    public class DieState : MainHumanState
    {
        public Action EndAction { get; set; }
        
        public override MainAnimationType MainAnimationType => MainAnimationType.Die;

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
