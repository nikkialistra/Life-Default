using System;

namespace General.Questing
{
    public interface IObjective
    {
        event Action<string> Update;
        
        void Activate();
        void Deactivate();

        string ToText();
    }
}
