﻿using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Colonists.Colonist.BehaviorNodes
{
    public class ResetBehavior : Action
    {
        public SharedBool NewCommand;

        public ColonistMeshAgent ColonistMeshAgent;
        public ColonistGatherer ColonistGatherer;

        public override TaskStatus OnUpdate()
        {
            NewCommand.Value = false;

            ColonistMeshAgent.StopMoving();
            ColonistGatherer.StopGathering();

            return TaskStatus.Success;
        }
    }
}
