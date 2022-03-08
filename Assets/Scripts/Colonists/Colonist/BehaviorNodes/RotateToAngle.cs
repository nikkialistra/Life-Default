using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Colonists.Colonist.BehaviorNodes
{
    public class RotateToAngle : Action
    {
        public SharedFloat Angle;

        public ColonistMeshAgent ColonistMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;
            ColonistMeshAgent.RotationEnd += OnRotationEnd;
            ColonistMeshAgent.RotateToAngle(Angle.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            ColonistMeshAgent.StopRotating();
        }

        private void OnRotationEnd()
        {
            ColonistMeshAgent.RotationEnd -= OnRotationEnd;
            _finished = true;
        }
    }
}
