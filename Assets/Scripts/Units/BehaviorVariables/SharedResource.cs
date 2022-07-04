using System;
using BehaviorDesigner.Runtime;
using ResourceManagement;

namespace Units.BehaviorVariables
{
    [Serializable]
    public class SharedResource : SharedVariable<Resource>
    {
        public static implicit operator SharedResource(Resource value)
        {
            return new SharedResource { Value = value };
        }
    }
}
