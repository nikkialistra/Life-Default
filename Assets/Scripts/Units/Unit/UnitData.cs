using System;
using UnityEngine;

namespace Units.Unit
{
    [Serializable]
    public struct UnitData
    {
        public string Id;

        public UnitType.UnitType Type;

        public Vector3 Position;
        public Quaternion Rotation;
    }
}
