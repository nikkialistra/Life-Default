using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Entities.BehaviorVariables
{
    [Serializable]
    public class SharedPositions : SharedVariable<Queue<Vector3>>
    {
        public static implicit operator SharedPositions(Queue<Vector3> value)
        {
            return new SharedPositions { Value = value };
        }
    }
}
