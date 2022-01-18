using System;
using Entities.Entity;

namespace Units.Unit.UnitType
{
    [Serializable]
    public class SpecsPerEntityType
    {
        public EntityType EntityType;

        public int Damage;
        public float DamageSpeedPerSecond;
    }
}
