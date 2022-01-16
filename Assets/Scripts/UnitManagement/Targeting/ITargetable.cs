using System;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public interface ITargetable
    {
        public event Action<ITargetable> TargetReach;

        public GameObject GameObject { get; }
        public Vector3 Position { get; }

        bool TryAcceptTargetWithPosition(Target target, Vector3 position);
    }
}
