using System;

namespace General.Questing.Objectives
{
    public interface IObjective
    {
        event Action<string> Update;
        
        void Activate(QuestServices questServices);
        void Deactivate();

        string ToText();
    }
}
