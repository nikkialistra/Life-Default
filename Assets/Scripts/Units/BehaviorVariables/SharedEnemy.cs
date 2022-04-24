using System;
using BehaviorDesigner.Runtime;
using Enemies;

namespace Units.BehaviorVariables
{
    [Serializable]
    public class SharedEnemy : SharedVariable<Enemy>
    {
        public static implicit operator SharedEnemy(Enemy value)
        {
            return new SharedEnemy{ Value = value };
        }
    }
}
