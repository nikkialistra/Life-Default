using BehaviorDesigner.Runtime.Tasks;
using Entities.Entity;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class InteractWithResource : Action
    {
        public SharedResource Resource;

        public UnitMeshAgent UnitMeshAgent;
        public UnitRole UnitRole;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;

            if (!UnitRole.CanInteractWith(Resource.Value))
            {
                _finished = true;
            }
            else
            {
                UnitRole.InteractWith(Resource.Value, OnInteractionFinish);
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
