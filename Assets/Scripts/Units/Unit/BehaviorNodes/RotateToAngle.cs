using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Units.Unit.BehaviorNodes
{
    public class RotateToAngle : Action
    {
        public SharedFloat Angle;

        public UnitMeshAgent UnitMeshAgent;

        private bool _finished;

        public override void OnStart()
        {
            _finished = false;
            UnitMeshAgent.RotationEnd += OnRotationEnd;
            UnitMeshAgent.RotateToAngle(Angle.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return _finished ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            UnitMeshAgent.StopRotating();
        }

        private void OnRotationEnd()
        {
            UnitMeshAgent.RotationEnd -= OnRotationEnd;
            _finished = true;
        }
    }
}
