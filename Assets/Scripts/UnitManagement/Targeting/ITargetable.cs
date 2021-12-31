using System;

namespace UnitManagement.Targeting
{
    public interface ITargetable
    {
        public event Action<ITargetable, Target> TargetReach;
        
        bool TryAcceptTarget(Target target);
    }
}