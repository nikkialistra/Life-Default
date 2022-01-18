using System;
using Entities;
using Entities.Entity;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public interface IOrderable
    {
        public event Action<IOrderable> DestinationReach;

        public GameObject GameObject { get; }
        public Vector3 Position { get; }

        bool TryOrderToEntityWithPosition(Entity entity, Vector3 position);
        bool TryOrderToPosition(Vector3 position);
    }
}
