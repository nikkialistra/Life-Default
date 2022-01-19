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
        private UnitClass _unitClass;

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

            if (!CanInteract(target))
            {
                Stopped(false);
                return;
            }

            Interact();
        }

        private bool CanInteract(Entity entity)
        {
            _entity = entity;
            _unitClass = Blackboard.Get<UnitClass>(_unitClassKey);

            return _unitClass.CanInteractWith(_entity);
        }

        private void Interact()
        {
            _unitClass.InteractWith(_entity, OnInteractionFinish);
        }

        private void OnInteractionFinish()
        {
            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
