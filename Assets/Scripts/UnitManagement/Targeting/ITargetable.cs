using UnityEngine;

namespace UnitManagement.Targeting
{
    public interface ITargetable
    {
        bool TryAcceptPoint(GameObject point);
    }
}