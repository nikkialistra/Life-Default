using System;
using UnityEngine;

namespace Entities.Animations
{
    public interface IAnimations
    {
        void Initialize();
        void OnHit(Vector3 agentPosition);
        void OnDestroy(Vector3 agentPosition, Action onFinish);
    }
}
