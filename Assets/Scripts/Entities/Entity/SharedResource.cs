using BehaviorDesigner.Runtime;
using ResourceManagement;

namespace Entities.Entity
{
    [System.Serializable]
    public class SharedResource : SharedVariable<Resource>
    {
        public static implicit operator SharedResource(Resource value)
        {
            return new SharedResource { Value = value };
        }
    }
}
