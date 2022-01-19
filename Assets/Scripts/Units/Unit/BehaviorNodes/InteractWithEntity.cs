using System;
using Entities.Entity;
using NPBehave;
using Units.Unit.UnitType;

namespace Units.Unit.BehaviorNodes
{
    public class InteractWithEntity : Node
    {
        private readonly string _entityKey;
        private readonly string _unitClassKey;
        
        private Entity _entity;
        private UnitClass _unitClass;

        public InteractWithEntity(string entityKey, string unitClassKey) : base("StartActionOnTarget")
        {
            _entityKey = entityKey;
            _unitClassKey = unitClassKey;
        }
        
        protected override void DoStart()
        {
            var entity = Blackboard.Get<Entity>(_entityKey);
            
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            Blackboard.Unset(_entityKey);

            if (!CanInteract(entity))
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
