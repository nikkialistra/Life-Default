using System;
using BehaviorDesigner.Runtime;
using Enemies.Enemy;

namespace Entities.Entity
{
    [Serializable]
    public class SharedEnemy : SharedVariable<EnemyFacade>
    {
        public static implicit operator SharedEnemy(EnemyFacade value)
        {
            return new SharedEnemy { Value = value };
        }
    }
}
