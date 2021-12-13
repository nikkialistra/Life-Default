using UnityEngine;

namespace Kernel.Types
{
    public interface ISelectable
    {
        GameObject GameObject { get; }
        void OnSelect();
        void OnDeselect();
    }
}