using System;
using NPBehave;
using ResourceManagement;
using UnitManagement.Targeting;
using Units.Unit.UnitType;
using UnityEngine;

namespace Units.Unit.BehaviorNodes
{
    public class StartActionOnTarget : Node
    {
        private readonly string _targetKey;
        private readonly string _unitClassKey;
        
        private Entity _entity;

        public StartActionOnTarget(string targetKey, string unitClassKey) : base("StartActionOnTarget")
        {
            _targetKey = targetKey;
            _unitClassKey = unitClassKey;
        }
        
        protected override void DoStart()
        {
            var target = Blackboard.Get<TargetMark>(_targetKey).Target;

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!CanAct(target))
            {
                Stopped(false);
                return;
            }

            Act();
        }

        private bool CanAct(Target target)
        {
            if (!target.HasEntity)
            {
                return false;
            }

            _entity = target.Entity;
            var unitClass = Blackboard.Get<UnitClass>(_unitClassKey);

            return unitClass.CanInteractWith(_entity);
        }

        private void Act()
        {
            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
