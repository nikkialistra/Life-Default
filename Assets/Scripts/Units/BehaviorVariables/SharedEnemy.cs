using System;
using BehaviorDesigner.Runtime;
using Enemies;

namespace Units.BehaviorVariables
{
    [Serializable]
    public class SharedUnit : SharedVariable<Unit>
    {
        public static implicit operator SharedUnit(Unit value)
        {
            return new SharedUnit { Value = value };
        }
    }
}
