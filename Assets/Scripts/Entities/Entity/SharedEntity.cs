using BehaviorDesigner.Runtime;

namespace Entities.Entity
{
    [System.Serializable]
    public class SharedEntity : SharedVariable<Entity>
    {
        public static implicit operator SharedEntity(Entity value)
        {
            return new SharedEntity { Value = value };
        }
    }
}
