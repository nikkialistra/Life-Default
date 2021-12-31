using System;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public interface ITargetable
    {
        public event Action<ITargetable> TargetReach;

        public GameObject GameObject { get; }
        
        bool TryAcceptTarget(Target target);
    }
}