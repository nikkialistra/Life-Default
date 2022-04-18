using UnityEngine;

namespace Entities.Animations
{
    public interface IAnimations
    {
        void OnHit(Vector3 agentPosition);
        void OnDestroy(Vector3 agentPosition);
    }
}
