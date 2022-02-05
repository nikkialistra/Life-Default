using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;
using Units.Unit.UnitTypes;

namespace Units.Unit.BehaviorNodes
{
    public class InteractWithResource : Action
    {
        public SharedResource Resource;

        public UnitRole UnitRole;

        private bool _finished;
        private bool _failed;

        public override void OnStart()
        {
            _finished = false;
            _failed = false;

            if (!UnitRole.CanInteractWith(Resource.Value))
            {
                _failed = true;
            }
            else
            {
                UnitRole.InteractWith(Resource.Value, OnInteractionFinish);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_failed)
            {
                return TaskStatus.Failure;
            }

            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        private void OnInteractionFinish()
        {
            _finished = true;
        }
    }
}
