using UnityEngine;

namespace Kernel.Types
{
    public interface ITargetable
    {
        bool TryAcceptPoint(GameObject point);
    }
}