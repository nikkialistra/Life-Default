using Entities.Entity;
using NPBehave;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class InteractWithEntity : Node
    {
        private readonly string _entityKey;
        private readonly UnitClass _unitClass;

        private Entity _entity;

        public InteractWithEntity(string entityKey, UnitClass unitClass) : base("InteractWithEntity")
        {
            _entityKey = entityKey;
            _unitClass = unitClass;
        }

        protected override void DoStart()
        {
            if (!Blackboard.Isset(_entityKey))
            {
                Stopped(false);
                return;
            }

            var entity = Blackboard.Get<Entity>(_entityKey);

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
