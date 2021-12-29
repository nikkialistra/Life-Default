using System;
using Game.Units.UnitTypes;
using UnityEngine;

namespace Game.Units.Unit
{
    [Serializable]
    public struct UnitData
    {
        public string Id;
        
        public UnitType Type;
        
        public Vector3 Position;
        public Quaternion Rotation;
    }
}