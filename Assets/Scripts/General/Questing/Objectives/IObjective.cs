using System;

namespace General.Questing.Objectives
{
    public interface IObjective
    {
        event Action<string> Update;
        event Action<string> Complete;

        void Activate(QuestServices questServices);
        void Deactivate();

        string ToText();
    }
}
