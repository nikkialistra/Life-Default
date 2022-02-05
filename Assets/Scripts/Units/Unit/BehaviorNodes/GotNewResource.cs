﻿using BehaviorDesigner.Runtime.Tasks;
using Entities.BehaviorVariables;

namespace Units.Unit.BehaviorNodes
{
    public class GotNewResource : Conditional
    {
        public SharedResource Resource;

        public override TaskStatus OnUpdate()
        {
            return Resource.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
