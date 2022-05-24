using System;
using UnityEngine;

namespace Infrastructure.Settings
{
    [Serializable]
    public class VisibilityFieldsSettings
    {
        public Vector3 TargetPositionCorrection = Vector3.up * 1.5f;
        public float VisibilityFieldRecalculationTime = 0.2f;
        public LayerMask ObstacleMask;
    }
}
