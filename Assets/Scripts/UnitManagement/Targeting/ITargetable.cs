using System;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public interface ITargetable
    {
        public event Action<ITargetable, GameObject> TargetReach;
        
        bool TryAcceptPoint(GameObject point);
    }
}