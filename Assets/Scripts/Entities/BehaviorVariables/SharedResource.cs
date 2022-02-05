using BehaviorDesigner.Runtime;
using ResourceManagement;

namespace Entities.BehaviorVariables
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
