using System;

namespace General.Questing
{
    [Serializable]
    public class Objective
    {
        public ObjectiveType Type;
        public ObjectiveTarget Target;
        public int Value;

        public enum ObjectiveType
        {
            Collect,
            Kill
        }

        public enum ObjectiveTarget
        {
            Wood,
            Stone,
            Enemy
        }
    }
}
