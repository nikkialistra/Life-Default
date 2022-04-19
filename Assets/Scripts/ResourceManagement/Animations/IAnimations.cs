using System;
using UnityEngine;

namespace ResourceManagement.Animations
{
    public interface IAnimations
    {
        void OnHit(Vector3 agentPosition);
        void OnDestroy(Vector3 agentPosition, Action onFinish);
    }
}
