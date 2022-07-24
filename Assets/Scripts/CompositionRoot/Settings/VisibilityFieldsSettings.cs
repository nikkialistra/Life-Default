using System;
using UnityEngine;

namespace CompositionRoot.Settings
{
    [Serializable]
    public class VisibilityFieldsSettings
    {
        public Vector3 TargetPositionCorrection = Vector3.up * 1.5f;
        public float VisibilityFieldRecalculationTime = 0.2f;
        public LayerMask ObstacleMask;
    }
}
