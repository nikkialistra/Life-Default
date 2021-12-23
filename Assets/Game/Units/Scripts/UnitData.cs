using System;
using UnityEngine;

namespace Game.Units
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