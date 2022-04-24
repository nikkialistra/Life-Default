using System;
using BehaviorDesigner.Runtime;
using Colonists;

namespace Units.BehaviorVariables
{
    [Serializable]
    public class SharedColonist : SharedVariable<Colonist>
    {
        public static implicit operator SharedColonist(Colonist value)
        {
            return new SharedColonist { Value = value };
        }
    }
}
