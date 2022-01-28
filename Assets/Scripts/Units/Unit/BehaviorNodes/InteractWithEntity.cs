using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class InteractWithEntity : Action
    {
        public SharedEntity Entity;

        public UnitMeshAgent UnitMeshAgent;
        public UnitClass UnitClass;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            if (!UnitClass.CanInteractWith(Entity.Value))
            {
                _finished = true;
            }
            else
            {
                UnitClass.InteractWith(Entity.Value, OnInteractionFinish);
            }
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnInteractionFinish()
        {
            _finished = true;
        }
    }
}
