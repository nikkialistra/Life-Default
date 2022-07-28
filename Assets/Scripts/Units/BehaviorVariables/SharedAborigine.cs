using System;
using Aborigines;
using BehaviorDesigner.Runtime;

namespace Units.BehaviorVariables
{
    [Serializable]
    public class SharedAborigine : SharedVariable<Aborigine>
    {
        public static implicit operator SharedAborigine(Aborigine value)
        {
            return new SharedAborigine { Value = value };
        }
    }
}
