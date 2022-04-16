using UnityEngine;

namespace Entities.Animations
{
    public interface IAnimations
    {
        void OnExtraction(Vector3 agentPosition);
        void OnExhaustion(Vector3 agentPosition);
    }
}
