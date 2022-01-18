using System;
using Entities.Entity;
using NPBehave;
using Units.Unit.UnitType;

namespace Units.Unit.BehaviorNodes
{
    public class InteractWithEntity : Node
    {
        private readonly string _targetKey;
        private readonly string _unitClassKey;
        
        private Entity _entity;

        public InteractWithEntity(string targetKey, string unitClassKey) : base("StartActionOnTarget")
        {
            _targetKey = targetKey;
            _unitClassKey = unitClassKey;
        }
        
        protected override void DoStart()
        {
            var target = Blackboard.Get<Entity>(_targetKey);

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

        private bool CanAct(Entity entity)
        {
            _entity = entity;
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
